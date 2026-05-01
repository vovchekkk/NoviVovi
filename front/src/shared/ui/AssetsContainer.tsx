import {css} from '../../../styled-system/css'
import {vstack} from '../../../styled-system/patterns';
import EditorHeader from "./EditorHeader.tsx";
import {useCallback, useEffect, useState, useMemo} from "react";
import EmotionBlock from "./EmotionBlock.tsx";
import { charactersApi } from "../api/client";
import {zodResolver} from '@hookform/resolvers/zod';
import * as z from 'zod';
import {useForm, useFieldArray, useWatch} from 'react-hook-form';
import {formatTransformForBackend, formatTransformForFrontend, getImageDimensions} from "../../pages/Editor.tsx";

const DEFAULT_TRANSFORM = {
    x: 50,
    y: 50,
    width: 40,
    height: 40,
    scale: 1,
    rotation: 0,
    zIndex: 1,
};

const emotionSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1, 'Название обязательно'),
    imageFile: z.any().optional().nullable(),
    fileUrl: z.string().optional(),
    transform: z.object({
        x: z.number().default(50),
        y: z.number().default(50),
        width: z.number().default(40),
        height: z.number().default(40),
        scale: z.number().default(1),
        rotation: z.number().default(0),
        zIndex: z.number().default(1),
    }).default(DEFAULT_TRANSFORM)
});
const characterSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1, 'Имя обязательно').max(50, 'Слишком длинное имя'),
    nameColor: z.string().regex(/^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/, 'Некорректный цвет').optional(),
    emotions: z.array(emotionSchema)
});
type CharacterSchema = z.infer<typeof characterSchema>;

export type Character = {
    id: string;
    name: string;
    nameColor?: string;
    characterStates: Emotion[],
};
type Emotion = {
    id: string;
    name: string;
    fileUrl?: string;
    image: {
        id:string
    }
}

interface AssetsProps {
    novelId: string;
}

export default function AssetsContainer({novelId}: AssetsProps) {
    const [characters, setCharacters] = useState<Character[]>([]);
    const [selectedId, setSelectedId] = useState<string | null>(characters[0]?.id ?? null);
    const [emotions, setEmotions] = useState<Emotion[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [activeIndex, setActiveIndex] = useState(0);
    const selectedCharacter = characters.find(character => character.id === selectedId) ?? null;
    const {
        register,
        handleSubmit,
        reset,
        control,
        setValue,
        watch,
        formState: {errors, isSubmitting}
    } = useForm<CharacterSchema>({
        resolver: zodResolver(characterSchema),
        defaultValues: {name: '', nameColor: '#ffffff', emotions: []}
    });
    
    // Используем watch вместо useWatch для более надежного отслеживания
    const formEmotions = watch("emotions");
    const currentEmotion = formEmotions?.[activeIndex];
    
    const getEmotionPreviewUrl = (emotion: any) => {
        if (!emotion) return null;
        if (emotion.imageFile instanceof File) return URL.createObjectURL(emotion.imageFile);
        return emotion.fileUrl || null;
    };
    const {fields, append, remove} = useFieldArray({
        control,
        name: "emotions"
    });
    
    const previewUrl = useMemo(() => {
        if (!currentEmotion) return null;
        if (currentEmotion.imageFile instanceof File) {
            return URL.createObjectURL(currentEmotion.imageFile);
        }
        return currentEmotion.fileUrl || null;
    }, [currentEmotion?.imageFile, currentEmotion?.fileUrl, activeIndex]);

    useEffect(() => {
        if (selectedCharacter) {
            reset({
                name: selectedCharacter.name,
                nameColor: selectedCharacter.nameColor || '#ffffff'
            });
        }
    }, [selectedId, reset, selectedCharacter]);

    useEffect(() => {
        const fetchCharacters = async () => {
            try {
                setLoading(true);
                const {data} = await charactersApi.getAll(novelId);
                setCharacters(data);

                if (data.length > 0) {
                    setSelectedId(data[0].id);
                }
            } catch (error) {
                console.error(error);
                alert('Не удалось загрузить персонажей');
            } finally {
                setLoading(false);
            }
        };

        fetchCharacters();
    }, [novelId]);
    
    useEffect(() => {
        const loadCharacterData = async () => {
            if (!selectedId) return;

            try {
                setLoading(true);
                const [charRes, statesRes] = await Promise.all([
                    charactersApi.getById(novelId, selectedId),
                    charactersApi.getStates(novelId, selectedId)
                ]);
                const mappedEmotions = (statesRes.data || []).map((state: any) => ({
                    id: state.id,
                    name: state.name,
                    fileUrl: state.image?.url || "",
                    imageId: state.image?.id || state.imageId,
                    imageFile: null,
                    transform: formatTransformForFrontend(state.localTransform || DEFAULT_TRANSFORM)
                }));
                reset({
                    name: charRes.data.name,
                    nameColor: charRes.data.nameColor || '#ffffff',
                    emotions: mappedEmotions
                });
            } catch (e) {
                console.error('Ошибка загрузки:', e);
            } finally {
                setLoading(false);
            }
        };

        loadCharacterData();
    }, [selectedId, reset, novelId]);
    const onSave = async (formData: CharacterSchema) => {
        if (!selectedId) return;

        // Проверяем, что у всех новых эмоций есть изображения
        const newEmotionsWithoutImage = formData.emotions.filter(
            emotion => !emotion.id && !emotion.imageFile && !(emotion as any).imageId
        );

        if (newEmotionsWithoutImage.length > 0) {
            const names = newEmotionsWithoutImage.map(e => `"${e.name}"`).join(', ');
            alert(`Невозможно сохранить новые эмоции без изображений: ${names}\n\nЗагрузите изображения для этих эмоций или удалите их.`);
            return;
        }

        try {
            setSaving(true);

            await charactersApi.patch(novelId, selectedId, {
                name: formData.name,
                nameColor: formData.nameColor,
            });

            const emotionPromises = formData.emotions.map(async (emotion, index) => {
                let currentImageId = (emotion as any).imageId;
                if (emotion.imageFile) {
                    const file = emotion.imageFile;
                    const dimensions = await getImageDimensions(file);

                    const uploadRequest = {
                        name: file.name,
                        type: 'Character',
                        format: file.type.split('/')[1],
                        size: {
                            width: dimensions.width.toString(),
                            height: dimensions.height.toString(),
                        },
                    };

                    const {data: uploadData} = await charactersApi.uploadImage(
                        novelId,
                        uploadRequest
                    );

                    await charactersApi.uploadToUrl(uploadData.uploadUrl, file);

                    currentImageId = uploadData.imageId;
                }

                const statePayload = {
                    name: emotion.name,
                    description: null,
                    imageId: currentImageId,
                    localTransform: formatTransformForBackend(emotion.transform) || DEFAULT_TRANSFORM
                };

                if (emotion.id) {
                    return charactersApi.updateState(novelId, selectedId, emotion.id, statePayload);
                } else {
                    return charactersApi.createState(novelId, selectedId, statePayload);
                }
            });

            await Promise.all(emotionPromises);

            // Перезагружаем данные персонажа с бэкенда, чтобы получить id для новых эмоций
            const [charRes, statesRes] = await Promise.all([
                charactersApi.getById(novelId, selectedId),
                charactersApi.getStates(novelId, selectedId)
            ]);
            const mappedEmotions = (statesRes.data || []).map((state: any) => ({
                id: state.id,
                name: state.name,
                fileUrl: state.image?.url || "",
                imageId: state.image?.id || state.imageId,
                imageFile: null,
                transform: formatTransformForFrontend(state.localTransform) || DEFAULT_TRANSFORM
            }));
            reset({
                name: charRes.data.name,
                nameColor: charRes.data.nameColor || '#ffffff',
                emotions: mappedEmotions
            });
            
            // Сбрасываем activeIndex, если он выходит за пределы массива
            if (activeIndex >= mappedEmotions.length) {
                setActiveIndex(Math.max(0, mappedEmotions.length - 1));
            }

            alert('Все данные и изображения сохранены!');
            setCharacters(prev => prev.map(c => c.id === selectedId ? {...c, name: formData.name} : c));

        } catch (error) {
            console.error("Ошибка при сохранении:", error);
            alert('Произошла ошибка при сохранении данных.');
        } finally {
            setSaving(false);
        }
    };
    const createCharacter = async () => {
        try {
            const {data: newChar} = await charactersApi.create(novelId, {
                name: 'Новый персонаж',
                nameColor: '#ffffff',
            })
            setCharacters(prev => [...prev, newChar]);
            setSelectedId(newChar.id);
        } catch (error) {
            console.error(error);
            alert('Не удалось создать персонажа');
        }
    }
    const deleteCharacter = async (id: string) => {
        if (!confirm('Удалить персонажа и все его эмоции?'))
            return;
        try {
            await charactersApi.delete(novelId, id);
            setCharacters(prev => prev.filter(c => c.id !== id));
            if (selectedId === id) {
                setSelectedId(characters[0]?.id ?? null);
            }
        } catch (error) {
            console.error(error);
        }
    }
    const addEmotion = async (name: string, file: File | null) => {

        if (!selectedId)
            return;
        const formData = new FormData();
        if (file)
            formData.append('file', file);
        try {
            const {data: newEmotion} = await api.post<Emotion>(
                `novels/${novelId}/characters/${selectedId}/states`,
                formData,
                {
                    headers: {'Content-Type': 'multipart/form-data'}
                }
            )
            setEmotions(prev => [...prev, newEmotion]);
        } catch (error) {
            console.error(error);
        }
    }

    return (
        <div className={css({
            height: '100%',
            background: '#775D68',
            display: 'flex',
            flexDirection: 'column',
            width: '100%',
            overflow: 'visible',
        })}>
            <EditorHeader active="assets" novelId={novelId}/>
            <div className={css({
                backgroundColor: 'white',
                color: 'black',
                width: '100%',
                paddingTop: '20px',
                display: 'flex',
                flexDirection: 'row',
                gap: '10px',
                flex: 1,
                overflow: 'hidden',
            })}>
                <div className={css({
                    backgroundColor: '#DFC6D1',
                    color: 'black',
                    flex: 1,
                    minWidth: '400px',
                    maxWidth: '400px',
                    borderRadius: '12px',
                    display: 'flex',
                    flexDirection: 'column',
                    overflow: 'hidden',
                })}>
                    <div className={css({
                        fontSize: '20px',
                        margin: '20px',
                        borderBottom: '1px solid black',
                        flexShrink: 0,
                    })}>
                        Персонажи
                    </div>
                    {loading && <p>Загрузка...</p>}
                    <div className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '10px',
                        alignItems: 'center',
                        flex: 1,
                        overflowY: 'auto',
                        padding: '10px 0',
                        '&::-webkit-scrollbar': {
                            width: '8px',
                        },
                        '&::-webkit-scrollbar-track': {
                            background: '#f1f1f1',
                            borderRadius: '10px',
                        },
                        '&::-webkit-scrollbar-thumb': {
                            background: '#888',
                            borderRadius: '10px',
                        },
                        '&::-webkit-scrollbar-thumb:hover': {
                            background: '#555',
                        },
                    })}>
                        {characters.map(character => (
                            <div
                                key={character.id}
                                className={css({
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '8px',
                                    width: '90%',
                                })}
                            >
                                <button
                                    onClick={() => setSelectedId(character.id)}
                                    className={css({
                                        display: 'flex',
                                        alignItems: 'stretch',
                                        gap: '4px',
                                        flex: 1,
                                        p: '4px',
                                        borderRadius: '12px',
                                        cursor: 'pointer',
                                        transition: 'all 0.2s',
                                        backgroundColor: character.id === selectedId ? '#705661' : '#F8EDEB',
                                        textAlign: 'left',
                                        _hover: {
                                            bg: '#775D68',
                                            color: 'background',
                                            borderColor: '#775D68',
                                            transform: 'translateY(-2px)',
                                            boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)',
                                        },
                                    })}>
                                    <div className={vstack({gap: '1px', alignItems: 'stretch', flex: 1})}>
                                        <p className={css({
                                            fontWeight: 'bold',
                                            minW: '0',
                                            fontSize: '20px',
                                            padding: '10px',
                                            backgroundColor: 'white',
                                            borderRadius: '12px',
                                            w: 'full',
                                        })}>
                                            {character.name}
                                        </p>
                                    </div>
                                </button>
                                <button
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        deleteCharacter(character.id);
                                    }}
                                    className={css({
                                        backgroundColor: '#ef4444',
                                        color: 'white',
                                        width: '32px',
                                        height: '32px',
                                        borderRadius: '8px',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                        cursor: 'pointer',
                                        flexShrink: 0,
                                        _hover: { backgroundColor: '#dc2626' },
                                    })}
                                    title="Удалить персонажа"
                                >
                                    ×
                                </button>
                            </div>
                        ))}
                    </div>
                    <div className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                        width: '100%',
                        py: '10px',
                    })}>
                        <button
                            onClick={createCharacter}
                            className={css({
                                mt: '8',
                                w: '90%',
                                py: '3',
                                bg: 'white',
                                cursor: 'pointer',
                                transition: 'all 0.2s',
                                _hover: {
                                    bg: '#705661',
                                    color: 'white',
                                    boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)',
                                    transform: 'translateY(-2px)'
                                },
                                borderRadius: '12px',
                                fontWeight: 'medium',
                            })}
                        >
                            + Добавить персонажа
                        </button>
                    </div>
                </div>
                <div className={css({
                    flex: 5,
                    display: 'flex',
                    flexDirection: 'column',
                    overflow: 'hidden',
                })}>
                    {selectedCharacter ? (
                        <form onSubmit={handleSubmit(onSave)}
                              className={css({
                                  backgroundColor: '#DFC6D1',
                                  color: 'black',
                                  borderRadius: '12px',
                                  display: 'flex',
                                  alignItems: 'start',
                                  flexDirection: 'column',
                                  flex: 1,
                                  overflow: 'hidden',
                              })}>
                            <div className={css({
                                display: 'flex',
                                width: '100%',
                                flex: 1,
                                overflow: 'hidden',
                            })}>
                                <div className={css({
                                    display: 'flex',
                                    flexDirection: 'column',
                                    borderRadius: '12px',
                                    padding: '5px',
                                    flex: 3,
                                    overflow: 'auto',
                                })}>
                                    <div className={css({
                                        backgroundColor: 'white',
                                        borderRadius: '12px',
                                        padding: '10px',
                                        margin: '10px',
                                        flex: 1,
                                    })}>
                                        Имя
                                        <div className={css({
                                                display: 'flex',
                                                maxHeight: '60%',
                                                width: '50%',
                                            }
                                        )}>
                                            <input
                                                {...register('name')}
                                                className={css({
                                                    border: '1px solid #ccc',
                                                    p: 2,
                                                    borderRadius: '8px',
                                                    width: '70%'
                                                })}
                                            />
                                            {errors.name && <p className={css({
                                                color: 'red',
                                                fontSize: '12px'
                                            })}>{errors.name.message}</p>}
                                        </div>
                                        <div style={{padding: 20}}>
                                            <label>
                                                Выберите цвет:{' '}
                                                <input
                                                    type="color"
                                                    {...register('nameColor')}
                                                    className={css({ml: 2, cursor: 'pointer', verticalAlign: 'middle'})}
                                                />
                                            </label>
                                        </div>
                                    </div>
                                    <div className={css({
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: '10px',
                                        justifyContent: 'space-between',
                                        backgroundColor: 'white',
                                        borderRadius: '12px',
                                        padding: '10px',
                                        margin: '10px',
                                        flex: 3,
                                        overflow: 'hidden',
                                    })}>
                                        <div className={css({
                                            display: 'flex',
                                            flexDirection: 'column',
                                            gap: '10px',
                                            flex: 1,
                                            overflow: 'hidden',
                                        })}>
                                            <div className={css({
                                                fontWeight: 'bold',
                                                fontSize: '20px',
                                                pl: '5px',
                                                flexShrink: 0,
                                            })}>
                                                Эмоции
                                            </div>
                                            <div className={css({
                                                display: 'flex',
                                                flexDirection: 'column',
                                                gap: '15px',
                                                overflowY: 'auto',
                                                flex: 1,
                                                paddingRight: '5px',
                                                '&::-webkit-scrollbar': {
                                                    width: '8px',
                                                },
                                                '&::-webkit-scrollbar-track': {
                                                    background: '#f1f1f1',
                                                    borderRadius: '4px',
                                                },
                                                '&::-webkit-scrollbar-thumb': {
                                                    background: '#888',
                                                    borderRadius: '4px',
                                                },
                                                '&::-webkit-scrollbar-thumb:hover': {
                                                    background: '#555',
                                                },
                                            })}>
                                                {fields.map((field, index) => {
                                                    const emotionData = formEmotions?.[index];
                                                    return (
                                                        <EmotionBlock
                                                            key={field.id}
                                                            index={index}
                                                            register={register}
                                                            setValue={setValue}
                                                            watch={watch}
                                                            control={control}
                                                            isActive={index === activeIndex}
                                                            onSelect={() => setActiveIndex(index)}
                                                            onRemove={() => remove(index)}
                                                            errors={errors.emotions?.[index]}
                                                            emotionId={emotionData?.id}
                                                            novelId={novelId}
                                                            characterId={selectedId}
                                                        />
                                                    );
                                                })}
                                            </div>
                                            <button
                                                type="button"
                                                onClick={() => {
                                                    append({
                                                        name: '',
                                                        imageFile: null,
                                                        transform: DEFAULT_TRANSFORM,
                                                    });
                                                    // Переключаемся на новую эмоцию
                                                    setActiveIndex(fields.length);
                                                }}
                                                className={css({
                                                    w: '30%',
                                                    py: '3',
                                                    bg: 'white',
                                                    border: '2px solid black',
                                                    _hover: {
                                                        bg: '#DFC6D1',
                                                        boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                                    },
                                                    borderRadius: '12px',
                                                    fontWeight: 'medium',
                                                })}
                                            >
                                                + Добавить эмоцию
                                            </button>
                                        </div>
                                    </div>

                                    <button type={'submit'} className={css({
                                        backgroundColor: 'white',
                                        borderRadius: '12px',
                                        width: '30%',
                                        marginLeft: '12px',
                                        fontSize: '20px',
                                        _hover: {
                                            boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                        }
                                    })}>Сохранить
                                    </button>
                                </div>
                                <div className={css({
                                    height: '100%',
                                    backgroundColor: '#333',
                                    borderRadius: '12px',
                                    padding: '10px',
                                    margin: '10px',
                                    flex: 1.5,
                                    position: 'relative',
                                    overflow: 'hidden',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    border: '2px solid #ccc'
                                })}>
                                    {currentEmotion ? (
                                        <>
                                            <div className={css({
                                                position: 'absolute',
                                                top: '10px',
                                                left: '10px',
                                                color: 'white',
                                                zIndex: 10,
                                                fontSize: '12px',
                                                backgroundColor: 'rgba(0,0,0,0.5)',
                                                padding: '2px 8px',
                                                borderRadius: '4px'
                                            })}>
                                                Превью: {currentEmotion.name || 'Без имени'}
                                            </div>

                                            {previewUrl ? (
                                                <img
                                                    src={previewUrl}
                                                    alt="Preview"
                                                    style={{
                                                        position: 'absolute',
                                                        left: `${currentEmotion.transform?.x ?? 50}%`,
                                                        top: `${currentEmotion.transform?.y ?? 50}%`,
                                                        width: `${currentEmotion.transform?.width ?? 40}%`,
                                                        height: `${currentEmotion.transform?.height ?? 40}%`,
                                                        transform: `translate(-50%, -50%) 
                                    scale(${currentEmotion.transform?.scale ?? 1}) 
                                    rotate(${currentEmotion.transform?.rotation ?? 0}deg)`,
                                                        zIndex: currentEmotion.transform?.zIndex ?? 1,
                                                        objectFit: 'fill'
                                                    }}
                                                />
                                            ) : (
                                                <div className={css({color: '#888'})}>Изображение не загружено</div>
                                            )}
                                        </>
                                    ) : (
                                        <div className={css({color: '#888', fontSize: '14px'})}>
                                            Выберите или добавьте эмоцию для превью
                                        </div>
                                    )}
                                </div>
                            </div>
                        </form>
                    ) : (
                        <p className={css({fontSize: '14px', color: '#555'})}>
                            Выберите персонажа для редактирования.
                        </p>
                    )}</div>
            </div>
        </div>
    )
}