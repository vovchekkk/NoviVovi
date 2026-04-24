import {css} from '../../../styled-system/css'
import {vstack} from '../../../styled-system/patterns';
import EditorHeader from "./EditorHeader.tsx";
import {useCallback, useEffect, useState} from "react";
import EmotionBlock from "./EmotionBlock.tsx";
import api from "../../api.tsx";
import {zodResolver} from '@hookform/resolvers/zod';
import * as z from 'zod';
import {useForm, useFieldArray} from 'react-hook-form';

const emotionSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1, 'Название обязательно'),
    imageFile: z.instanceof(File).optional().nullable(),
});
const characterSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1, 'Имя обязательно').max(50, 'Слишком длинное имя'),
    color: z.string().regex(/^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/, 'Некорректный цвет').optional(),
    emotions: z.array(emotionSchema)
});
type CharacterSchema = z.infer<typeof characterSchema>;

type Character = {
    id: string;
    name: string;
    color?: string;
};
type Emotion = {
    id: string;
    name: string;
    fileUrl?: string;
}
const initialCharacters: Character[] = [
    {id: '1', name: 'Анна'},
    {id: '2', name: 'Мария'},
    {id: '3', name: 'Борис'},
];
export default function AssetsContainer() {
    const [characters, setCharacters] = useState<Character[]>([]);
    const [selectedId, setSelectedId] = useState<string | null>(characters[0]?.id ?? null);
    const [emotions, setEmotions] = useState<Emotion[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
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
        defaultValues: {name: '', color: '#ffffff', emotions: []}
    });
    const {fields, append, remove} = useFieldArray({
        control,
        name: "emotions"
    });

    useEffect(() => {
        if (selectedCharacter) {
            reset({
                name: selectedCharacter.name,
                color: selectedCharacter.color || '#ffffff'
            });
        }
    }, [selectedId, reset, selectedCharacter]);

    useEffect(() => {
        const fetchCharacters = async () => {
            try {
                setLoading(true);
                const {data} = await api.get<Character[]>('/characters');
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
    }, []);
    useEffect(() => {
        const loadCharacterData = async () => {
            if (!selectedId) return;

            try {
                setLoading(true);
                const [charRes, emotionsRes] = await Promise.all([
                    api.get<Character>(`/characters/${selectedId}`),
                    api.get<Emotion[]>(`/characters/${selectedId}/emotions`)
                ]);
                reset({
                    name: charRes.data.name,
                    color: charRes.data.color || '#ffffff',
                    emotions: emotionsRes.data || []
                });
            } catch (e) {
                console.error('Ошибка загрузки:', e);
            } finally {
                setLoading(false);
            }
        };

        loadCharacterData();
    }, [selectedId, reset]);
    const onSave = async (formData: CharacterSchema) => {
        if (!selectedId) return;
        try {
            await api.patch(`characters/${selectedId}`, formData);
            setCharacters(prev =>
                prev.map(c => c.id === selectedId ? {...c, ...formData} : c)
            );
            alert('Сохранено!');
        } catch (error) {
            console.error(error);
            alert('Ошибка при сохранении');
        }
    };
    const createCharacter = async () => {
        try {
            const {data: newChar} = await api.post<Character>('/characters', {
                name: 'Новый персонаж'
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
            await api.delete(`characters/${id}`);
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
            formData.append('file', file); //имя параметра должно совпадать с бэком
        try {
            const {data: newEmotion} = await api.post<Emotion>(
                `characters/${selectedId}/emotions`,
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
            minHeight: '100vh',
            background: '#775D68',
            display: 'flex',
            flexDirection: 'column',
            width: '100%',
            gap: '0px',
            flex: 1,
            height: '100vh',
        })}>
            <EditorHeader active="assets"/>
            <div className={css({
                backgroundColor: 'white',
                height: '100vh',
                color: 'black',
                width: '100%',
                paddingTop: '20px',
                display: 'flex',
                flexDirection: 'row',
                gap: '10px',
                flex: 4,
                overflow: 'hidden'
            })}>
                <div className={css({
                    backgroundColor: '#DFC6D1',
                    color: 'black',
                    flex: 1,
                    minWidth: '400px',
                    borderRadius: '12px',
                })}>
                    <div className={css({
                        fontSize: '20px',
                        margin: '20px',
                        borderBottom: '1px solid black',
                    })}>
                        Персонажи
                    </div>
                    {loading && <p>Загрузка...</p>}
                    <div className={vstack({gap: '10px', alignItems: 'center'})}>
                        {characters.map(character => (
                            <button
                                key={character.id}
                                onClick={() => setSelectedId(character.id)}
                                className={css({
                                    display: 'flex',
                                    alignItems: 'stretch',
                                    gap: '4px',
                                    width: '90%',
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
                        ))}
                    </div>
                    <div className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    })}>
                        <button
                            onClick={createCharacter}
                            className={css({
                                mt: '8',
                                w: '90%',
                                py: '3',
                                bg: 'white',
                                _hover: {
                                    bg: '#705661', color: 'white',
                                    boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
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
                    maxWidth: '100%',
                    height: 'full',
                    display: 'flex',
                    flexDirection: 'column'
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
                              })}>
                            <div className={css({
                                display: 'flex',
                                width: '100%',
                                flex: 1
                            })}>
                                <div className={css({
                                    display: 'flex',
                                    flexDirection: 'column',
                                    borderRadius: '12px',
                                    padding: '5px',
                                    flex: 3,
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
                                                    {...register('color')}
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
                                    })}>
                                        <div className={css({
                                            display: 'flex',
                                            flexDirection: 'column',
                                            gap: '10px',
                                        })}>
                                            <div className={css({
                                                fontWeight: 'bold',
                                                fontSize: '20px',
                                                pl: '5px'
                                            })}>
                                                Эмоции
                                            </div>
                                            <div className={css({ display: 'flex', flexDirection: 'column', gap: '15px' })}>
                                                {fields.map((field, index) => (
                                                    <EmotionBlock
                                                        key={field.id}
                                                        index={index}
                                                        register={register}
                                                        setValue={setValue}
                                                        watch={watch}
                                                        onRemove={() => remove(index)}
                                                        errors={errors.emotions?.[index]}
                                                    />
                                                ))}
                                            </div>
                                            <button
                                                type="button"
                                                onClick={() => append({name: 'Новая эмоция'})}
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
                                    backgroundColor: 'white',
                                    borderRadius: '12px',
                                    padding: '10px',
                                    margin: '10px',
                                    flex: 1,
                                })}>
                                    Фото
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