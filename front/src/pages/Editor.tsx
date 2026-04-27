import Header from "../shared/ui/Header.tsx";
import {css} from '../../styled-system/css'
import {useEffect, useRef, useState} from "react";
import EditorHeader from "../shared/ui/EditorHeader.tsx";
import Preview from "../shared/ui/Preview.tsx";
import BlockPanel from "../shared/ui/BlockPanel.tsx";
import Selector from "../shared/ui/Selector.tsx";
import {Controller, useFieldArray, useForm, useWatch} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {z} from "zod";
import Modal from "../shared/ui/Modal.tsx";
import api from "../api.tsx";
import {useParams} from "react-router-dom";
import {vstack, hstack} from '../../styled-system/patterns';
import {LabelItem} from "../shared/ui/LabelItem.tsx";
import type {Character} from "../shared/ui/AssetsContainer.tsx";

export enum StepType {
    BACKGROUND = 'show_background',
    SHOW = 'show_character',
    HIDE = 'hide_character',
    REPLICA = 'replica',
    JUMP = 'jump',
    MENU = 'menu',
}

const transformSchema = z.object({
    x: z.number(),
    y: z.number(),
    width: z.number(),
    height: z.number(),
    scale: z.number(),
    rotation: z.number(),
    zIndex: z.number(),
});
export const stepDisplayNames: Record<StepType, string> = {
    [StepType.BACKGROUND]: 'Фон',
    [StepType.SHOW]: 'Появление',
    [StepType.HIDE]: 'Исчезновение',
    [StepType.REPLICA]: 'Реплика',
    [StepType.JUMP]: 'Переход',
    [StepType.MENU]: 'Выбор',
};
const backgroundStateSchema = z.object({
    imageId: z.string(),
    transform: transformSchema,
});

const characterStateSchema = z.object({
    characterId: z.string(),
    characterStateId: z.string(),
    transform: transformSchema,
});

const sceneStateSchema = z.object({
    background: backgroundStateSchema.nullable(),
    characters: z.array(characterStateSchema),
});
const baseStepSchema = z.object({
    id: z.string().min(1),
    state: sceneStateSchema,
});

const hideStepSchema = baseStepSchema.extend({
    type: z.literal('hide_character'),
    characterId: z.string().min(1, 'Выберите персонажа'),
});

const jumpStepSchema = baseStepSchema.extend({
    type: z.literal('jump'),
    targetId: z.string().min(1, 'Выберите следующую сцену'),
});

const showStepSchema = baseStepSchema.extend({
    type: z.literal('show_character'),
    characterId: z.string().min(1),
    characterStateId: z.string().min(1),
    transform: transformSchema,
});

const backgroundStepSchema = baseStepSchema.extend({
    type: z.literal('show_background'),
    imageId: z.string().min(1),
    transform: transformSchema,
});

const replicaStepSchema = baseStepSchema.extend({
    type: z.literal('replica'),
    characterId: z.string().min(1),
    text: z.string().min(1, 'Введите текст реплики'),
});

const choiceStepSchema = baseStepSchema.extend({
    type: z.literal('menu'),
    name: z.string(),
    text: z.string(),
    menuRequest: z.object({
        id: z.string(),
        name: z.string().nullable(),
        text: z.string().nullable(),
        choices: z.array(z.object({
            id: z.string(),
            name: z.string(),
            text: z.string(),
            targetLabelId: z.string(),
        })),
    }),
});

export const stepSchema = z.discriminatedUnion('type', [
    hideStepSchema,
    jumpStepSchema,
    showStepSchema,
    backgroundStepSchema,
    replicaStepSchema,
    choiceStepSchema,
]);

export type Step = z.infer<typeof stepSchema>;
export type Label = {
    id: string;
    name: string;
}

type CharacterOption = {
    id:string,
    name:string,
    states:{
        id: string;
        name: string;
    }[],
}
const CHARACTERS = ['Анна', 'Мария', 'Борис'];
const SCENES = ['Парк', 'Улица', 'Дом'];
type StepFormProps = {
    control: any;
    errors: any;
    register: any;
    characterOptions: CharacterOption[];
    labelOptions: string[];
    setValue:any;
    novelId:string;
};

function HideStepForm({control, errors, characterOptions}: StepFormProps) {
    const options = characterOptions.map(ch => ch.name);
    return (
        <div>
            <Controller
                control={control}
                name="characterId"
                render={({field}) => <Selector title="Персонаж" options={options} {...field} />}
            />
            {errors.characterId && <p className={css({color: 'red'})}>{errors.characterId.message}</p>}
        </div>
    );
}

function JumpStepForm({control, errors, labelOptions}: StepFormProps) {
    return (
        <div>
            <Controller
                control={control}
                name="targetId"
                render={({field}) => <Selector title="Сцена" options={labelOptions} {...field} />}
            />
            {errors.targetId && <p className={css({color: 'red'})}>{errors.targetId.message}</p>}
        </div>
    );
}

function ShowStepForm({ control, errors, characterOptions, setValue }: StepFormProps) {
    const selectedCharacterId = useWatch({ control, name: 'characterId' });
    const previousCharacterIdRef = useRef<string | undefined>();
    const selectedCharacter = characterOptions.find(ch => ch.id === selectedCharacterId);
    const options = characterOptions.map(ch => ({
        value: ch.id,
        label: ch.name
    }));
    const stateOptions = (selectedCharacter?.states || []).map(state => ({
        value: state.id,
        label: state.name
    }));

    useEffect(() => {
        if (previousCharacterIdRef.current !== undefined && previousCharacterIdRef.current !== selectedCharacterId) {
            setValue('characterStateId', '');
        }

        previousCharacterIdRef.current = selectedCharacterId;
    }, [selectedCharacterId, setValue]);

    return (
        <div className={css({ display: 'flex', flexDirection: 'column', gap: '12px' })}>
            <div className={css({ display: 'flex', flexDirection: 'column', gap: '8px' })}>
                <Controller
                    control={control}
                    name="characterId"
                    render={({ field }) =>
                        <Selector title="Персонаж" options={options} {...field} />}
                />
                <Controller
                    control={control}
                    name="characterStateId"
                    render={({ field }) => (
                        <Selector
                            title="Состояние"
                            options={stateOptions}
                            value={field.value}
                            onBlur={field.onBlur}
                            onChange={field.onChange}
                            disabled={!selectedCharacterId}
                        />
                    )}
                />
            </div>

            <div className={css({
                padding: '12px',
                border: '1px solid #eee',
                borderRadius: '8px',
                backgroundColor: '#fafafa'
            })}>
                <div className={css({
                    fontSize: '12px',
                    fontWeight: 'bold',
                    marginBottom: '10px',
                    color: '#555',
                    textTransform: 'uppercase',
                    letterSpacing: '0.5px'
                })}>
                    Трансформация
                </div>

                <div className={css({
                    display: 'grid',
                    gridTemplateColumns: '1fr 1fr',
                    gap: '8px 12px'
                })}>
                    <CompactInput label="X" name="transform.x" control={control} />
                    <CompactInput label="Y" name="transform.y" control={control} />

                    <CompactInput label="W" name="transform.width" control={control} />
                    <CompactInput label="H" name="transform.height" control={control} />

                    <CompactInput label="Scale" name="transform.scale" control={control} step="0.1" />
                    <CompactInput label="Rot°" name="transform.rotation" control={control} />

                    <div className={css({ gridColumn: 'span 2' })}>
                        <CompactInput label="Z-Index" name="transform.zIndex" control={control} />
                    </div>
                </div>
            </div>

            {(errors.characterId || errors.characterStateId) && (
                <p className={css({ color: 'red', fontSize: '12px' })}>Заполните обязательные поля</p>
            )}
        </div>
    );
}

function CompactInput({ label, name, control, step = "1" }: any) {
    return (
        <Controller
            name={name}
            control={control}
            render={({ field }) => (
                <div className={css({
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    gap: '6px'
                })}>
                    <span className={css({
                        color: '#888',
                        fontSize: '11px',
                        fontWeight: '500',
                        width: '30px'
                    })}>
                        {label}
                    </span>
                    <input
                        type="number"
                        step={step}
                        className={css({
                            width: '100%',
                            padding: '4px 6px',
                            border: '1px solid #ccc',
                            borderRadius: '4px',
                            fontSize: '12px',
                            outline: 'none',
                            ':focus': { borderColor: '#007bff' }
                        })}
                        {...field}
                        onChange={(e) => field.onChange(Number(e.target.value))}
                    />
                </div>
            )}
        />
    );
}

function BackgroundStepForm({ control, errors, setValue, novelId }: StepFormProps) {
    const [isUploading, setIsUploading] = useState(false);
    const currentImageId = useWatch({
        control,
        name: 'background.imageId'
    });

    const getImageDimensions = (file: File): Promise<{ width: number; height: number }> => {
        return new Promise((resolve) => {
            const reader = new FileReader();
            reader.onload = (e) => {
                const img = new Image();
                img.onload = () => resolve({ width: img.width, height: img.height });
                img.src = e.target?.result as string;
            };
            reader.readAsDataURL(file);
        });
    };

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file) return;

        setIsUploading(true);

        try {
            const dimensions = await getImageDimensions(file);

            const request = {
                name: file.name,
                type: 'Background',
                format: file.type.split('/')[1],
                size: {
                    width: dimensions.width.toString(),
                    height: dimensions.height.toString(),
                },
            };

            const response = await api.post(`novels/${novelId}/images/upload-url`, request);
            const { imageId, uploadUrl, viewUrl } = response.data;
            console.log(uploadUrl)
            console.log('imageId:', imageId);

            if (uploadUrl) {
                await api.put(uploadUrl, file, {
                    headers: { 'Content-Type': file.type }
                });
            }

            if (imageId) {
                setValue('background.imageId', imageId);
                alert('Изображение загружено и сохранено в облако!');
            }

        } catch (error) {
            console.error("Ошибка загрузки:", error);
            alert('Ошибка при передаче файла в облако.');
        } finally {
            setIsUploading(false);
        }
    };

    return (
        <div className={css({ display: 'flex', flexDirection: 'column', gap: '16px' })}>
            <div className={css({ display: 'flex', flexDirection: 'column', gap: '8px' })}>
                <label className={css({ fontWeight: 'bold', fontSize: '14px' })}>Изображение фона</label>

                <div className={css({
                    padding: '20px',
                    border: '2px dashed #ccc',
                    borderRadius: '8px',
                    textAlign: 'center',
                    backgroundColor: '#fafafa',
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    gap: '10px'
                })}>
                    {currentImageId ? (
                        <div className={css({ fontSize: '12px', color: '#28a745' })}>
                            ✅ ID в базе: {currentImageId}
                        </div>
                    ) : (
                        <div className={css({ fontSize: '12px', color: '#666' })}>
                            Фон еще не загружен
                        </div>
                    )}

                    <label className={css({
                        padding: '8px 16px',
                        backgroundColor: '#333',
                        color: 'white',
                        borderRadius: '6px',
                        fontSize: '13px',
                        cursor: isUploading ? 'not-allowed' : 'pointer',
                        '&:hover': { backgroundColor: '#000' }
                    })}>
                        {isUploading ? 'Загрузка...' : 'Выбрать и загрузить'}
                        <input
                            type="file"
                            accept="image/*"
                            onChange={handleFileUpload}
                            disabled={isUploading}
                            className={css({ display: 'none' })}
                        />
                    </label>
                </div>
                {errors.background?.imageId && (
                    <p className={css({ color: 'red', fontSize: '12px' })}>
                        {errors.background.imageId.message || 'Нужно загрузить фон'}
                    </p>
                )}
            </div>
            <div className={css({
                padding: '12px',
                border: '1px solid #eee',
                borderRadius: '8px',
                backgroundColor: '#fff'
            })}>
                <div className={css({ fontSize: '11px', fontWeight: 'bold', marginBottom: '10px', color: '#888', textTransform: 'uppercase' })}>
                    Позиция фона
                </div>

                <div className={css({ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '8px 12px' })}>
                    <CompactInput label="X" name="background.transform.x" control={control} />
                    <CompactInput label="Y" name="background.transform.y" control={control} />
                    <CompactInput label="W" name="background.transform.width" control={control} />
                    <CompactInput label="H" name="background.transform.height" control={control} />
                    <CompactInput label="Scale" name="background.transform.scale" control={control} step="0.1" />
                    <CompactInput label="Rot°" name="background.transform.rotation" control={control} />
                    <div className={css({ gridColumn: 'span 2' })}>
                        <CompactInput label="Z-Index" name="background.transform.zIndex" control={control} />
                    </div>
                </div>
            </div>
        </div>
    );
}

function ReplicaStepForm({control, errors, register}: StepFormProps) {
    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '16px'})}>
            <Controller
                control={control}
                name="characterId"
                render={({field}) => <Selector title="Персонаж" options={CHARACTERS} {...field} />}
            />
            {errors.characterId &&
                <p className={css({color: 'red', fontSize: '13px'})}>{errors.characterId.message}</p>}

            <div className={css({
                display: "flex",
                flexDirection: "column",
                gap: '10px',
                width: '300px',
                margin: '0 auto',
            })}>
                <label className={css({fontSize: '18px', textAlign: 'left'})}>Текст реплики</label>
                <textarea
                    {...register('text')}
                    className={css({
                        width: '300px',
                        minHeight: '120px',
                        padding: '12px',
                        borderRadius: '8px',
                        backgroundColor: 'white',
                        border: '1px solid black',
                    })}
                />
            </div>
            {errors.text && <p className={css({color: 'red', fontSize: '13px'})}>{errors.text.message}</p>}
        </div>
    );
}

function ChoiceStepForm({ control, errors, labelOptions }: StepFormProps) {
    const { fields, append, remove } = useFieldArray({
        control,
        name: "choices"
    });

    return (
        <div className={css({ display: 'flex', flexDirection: 'column', gap: '16px', width: '100%', maxWidth: '420px', margin: '0 auto' })}>

            <div className={css({ display: 'flex', flexDirection: 'column', gap: '8px', padding: '16px', border: '1px solid #eee', borderRadius: '8px', backgroundColor: '#fafafa' })}>
                <label className={css({ fontSize: '14px', fontWeight: 'bold', color: '#555' })}>Текст вопроса</label>
                <textarea
                    {...control.register('text')}
                    placeholder="Что увидит игрок?"
                    rows={4}
                />
                {errors.text && <p className={css({ color: 'red', fontSize: '12px' })}>{String(errors.text.message)}</p>}
            </div>

            <div className={css({ display: 'flex', flexDirection: 'column', gap: '12px', padding: '16px', border: '1px solid #eee', borderRadius: '8px', backgroundColor: '#fafafa' })}>
                <div className={css({ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '12px' })}>
                    <label className={css({ fontSize: '12px', fontWeight: 'bold', color: '#555', textTransform: 'uppercase', letterSpacing: '0.5px' })}>Варианты ответов</label>
                    <button
                        type="button"
                        onClick={() => append({ text: '', transition: { targetLabelId: '' } })}
                        className={css({
                            padding: '8px 12px',
                            backgroundColor: '#705661',
                            color: 'white',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            fontSize: '12px',
                            fontWeight: '600',
                            transition: 'background-color 0.2s',
                            _hover: {
                                backgroundColor: '#5e4a52'
                            }
                        })}
                    >
                        + Добавить
                    </button>
                </div>

                {fields.map((field, index) => (
                    <div key={field.id} className={css({
                        padding: '14px',
                        border: '1px solid #e6d9df',
                        borderRadius: '8px',
                        backgroundColor: '#fff',
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '12px',
                        position: 'relative'
                    })}>
                        <button
                            type="button"
                            onClick={() => remove(index)}
                            className={css({
                                position: 'absolute',
                                top: '10px',
                                right: '10px',
                                border: 'none',
                                background: 'none',
                                cursor: 'pointer',
                                color: '#a54f67',
                                fontWeight: 'bold',
                                fontSize: '16px',
                                lineHeight: 1
                            })}
                        >
                            ✕
                        </button>

                        <div className={css({ display: 'flex', flexDirection: 'column', gap: '8px' })}>
                            <label className={css({ fontSize: '12px', fontWeight: 'bold', color: '#666' })}>Текст кнопки</label>
                            <input
                                {...control.register(`choices.${index}.text` as const)}
                                className={inputStyle}
                                placeholder="Текст ответа"
                            />
                        </div>

                        <div className={css({ display: 'flex', flexDirection: 'column', gap: '8px' })}>
                            <Controller
                                control={control}
                                name={`choices.${index}.transition.targetLabelId` as const}
                                render={({ field: selectField }) => (
                                    <Selector
                                        title="Переход на сцену"
                                        options={labelOptions}
                                        {...selectField}
                                    />
                                )}
                            />
                        </div>
                    </div>
                ))}

                {fields.length === 0 && (
                    <p className={css({ fontSize: '12px', color: '#888', textAlign: 'center', padding: '12px 0' })}>
                        Добавьте хотя бы один вариант выбора
                    </p>
                )}
            </div>
        </div>
    );
}

// Стили для переиспользования
const inputStyle = css({
    width: '100%',
    padding: '10px',
    borderRadius: '8px',
    backgroundColor: 'white',
    border: '1px solid black',
    fontSize: '14px'
});

const compactInputStyle = css({
    width: '100%',
    padding: '6px 8px',
    borderRadius: '4px',
    border: '1px solid #ccc',
    fontSize: '13px'
});

export default function Editor() {
    const {novelId} = useParams<{ novelId: string }>() ?? 0;
    const [isOpen, setIsOpen] = useState(false);
    const [steps, setSteps] = useState<Step[]>([]);
    const [labels, setLabels] = useState<Label[]>([]);
    const [imageUrl, setImageUrl] = useState('');
    const [selectedLabelId, setSelectedLabelId] = useState<string | null>(labels[0]?.id ?? null);
    const [selectedStepIndex, setSelectedStepIndex] = useState(0);
    const [selectedId, setSelectedId] = useState<string | null>(steps[0]?.id ?? null);
    const currentStep = steps[selectedStepIndex];
    const [loading, setLoading] = useState(true);
    const [labelName, setLabelName] = useState(' ');
    const [isLabelOpen, setIsLabelOpen] = useState(false);
    const [characterOptions, setCharacterOptions] = useState<CharacterOption[]>([]);
    const {
        register,
        handleSubmit,
        reset,
        control,
        setValue,
        formState: {errors},
    } = useForm<Step>({
        resolver: zodResolver(stepSchema),
        mode: 'onChange',
        defaultValues: {
            characterId: '',
            characterStateId: '',
            background: {
                imageId: '',
                transform: {
                    x: 0,
                    y: 0,
                    width: 100,
                    height: 100,
                    scale: 1,
                    rotation: 0,
                    zIndex: 1
                }
            },
            characters: []
        }
    });

    useEffect(() => {
        const fetchCharacterNames = async () => {
            try {
                const {data} = await api.get<Character[]>(`novels/${novelId}/characters`);
                setCharacterOptions(data.map(ch => ({
                    id:ch.id,
                    name:ch.name,
                    states:ch.characterStates.map(st => ({
                        id: st.id,
                        name: st.name,
                    })),
                })))
            } catch (error) {
                console.log(error);
            }
        }
        fetchCharacterNames();
    }, [novelId]);

    useEffect(() => {
        if (currentStep) {
            reset(currentStep);
        }
    }, [selectedStepIndex, currentStep, reset]);
    useEffect(() => {
        const loadStepData = async () => {
            if (!selectedId) return;

            try {
                setLoading(true);
                const {data} = await api.get<Step>(`/steps/${selectedId}`);
                const baseData = ({
                    id: data.id,
                    type: data.type,
                });
                switch (data.type) {
                    case 'show':
                        reset({
                            ...baseData,
                            characterId: data.characterId,
                            characterStateId: data.characterStateId,
                            transform: data.transform,
                        });
                        break;

                    case 'hide':
                        reset({
                            ...baseData,
                            characterId: data.characterId,
                        });
                        break;

                    case 'background':
                        reset({
                            ...baseData,
                            imageId: data.imageId,
                            transform: data.transform,
                        });
                        break;

                    case 'jump':
                        reset({
                            ...baseData,
                            targetId: data.targetId,
                        });
                        break;

                    case 'replica':
                        reset({
                            ...baseData,
                            characterId: data.characterId,
                            text: data.text,
                        });
                        break;

                    case 'choice':
                        reset({
                            ...baseData,
                            name: data.name,
                            text: data.text,
                            menuRequest: data.menuRequest,
                        });
                        break;

                    default:
                        reset(baseData);
                }
            } catch (e) {
                console.error('Ошибка загрузки:', e);
            } finally {
                setLoading(false);
            }
        };

        loadStepData();
    }, [selectedId, reset]);
    useEffect(() => {
        const fetchLabels = async () => {
            try {
                setLoading(true);
                const {data} = await api.get<Label[]>(`/novels/${novelId}/labels`);
                setLabels(data);

                if (data.length > 0) {
                    setSelectedLabelId(data[0].id);
                }
            } catch (error) {
                console.error(error);
                alert('Не удалось загрузить персонажей');
            } finally {
                setLoading(false);
            }
        }
        fetchLabels()
    }, [novelId]);
    useEffect(() => {
        const fetchSteps = async () => {
            if (!selectedLabelId || selectedLabelId === 'null') {
                setSteps([]);
                return;
            }
            try {
                setLoading(true);
                const {data} = await api.get<Step[]>(`novels/${novelId}/labels/${selectedLabelId}/steps`);
                setSteps(data);

                if (data.length > 0) {
                    setSelectedId(data[0].id);
                }
            } catch (error) {
                console.error(error);
                alert('Не удалось загрузить шаги');
            } finally {
                setLoading(false);
            }
        };

        fetchSteps();
    }, [selectedLabelId, novelId]);
    const onSave = async (data: any) => {
        const finalState = { ...(data.state || {}) };

        if (data.type === 'background' && data.background) {
            finalState.background = data.background;
        }

        if (data.type === 'show' && data.characterId) {
            const newChar = {
                characterId: data.characterId,
                characterStateId: data.characterStateId || null,
                transform: data.transform || { x: 40, y: 30, width: 25, height: 65, scale: 1, rotation: 0, zIndex: 20 }
            };

            finalState.characters = [
                ...(finalState.characters || []).filter((ch: any) => ch.characterId !== data.characterId),
                newChar
            ];
        }

        if (data.type === 'hide' && data.characterId) {
            finalState.characters = (finalState.characters || [])
                .filter((ch: any) => ch.characterId !== data.characterId);
        }

        const finalData = {
            ...data,
            state: finalState
        };

        try {
            let savedStep: Step;

            if (data.id) {
                const { data: updated } = await api.patch(`/steps/${data.id}`, finalData);
                savedStep = updated;
            }
            else {
                const { data: newStep } = await api.post<Step>(
                    `/novels/${novelId}/labels/${selectedLabelId}/steps`,
                    finalData
                );
                savedStep = newStep;
            }

            setSteps((prevSteps) =>
                prevSteps.map((step, index) =>
                    index === selectedStepIndex ? savedStep : step
                )
            );

            console.log('Шаг успешно сохранён:', savedStep);

        } catch (error) {
            console.error('Ошибка сохранения шага:', error);
            alert('Не удалось сохранить шаг');
        }
    };
    const addStep = (type: StepType) => {
        let tempStep: any = {
            id: null,
            type: '',
            state: {},
        };

        switch (type) {
            case 'show_background':
                tempStep.type = 'show_background';
                tempStep.transform = { x: 0, y: 0, width: 100, height: 100, scale: 1, rotation: 0, zIndex: 0 };
                break;

            case 'show_character':
                tempStep.type = 'show_character';
                tempStep.characterId = '';
                tempStep.characterStateId = '';
                tempStep.transform = { x: 50, y: 50, width: 25, height: 60, scale: 1, rotation: 0, zIndex: 10 };
                break;

            case 'hide_character':
                tempStep.type = 'hide_character';
                tempStep.characterId = '';
                break;

            case 'replica':
                tempStep.type = 'replica';
                tempStep.characterId = '';
                tempStep.text = '';
                break;

            case 'jump':
                tempStep.type = 'jump';
                tempStep.targetId = '';
                break;

            case 'menu':
                tempStep.type = 'menu';
                tempStep.name = '';
                tempStep.text = '';
                tempStep.menuRequest = {
                    id: `temp-menu-${Date.now()}`,
                    name: null,
                    text: null,
                    choices: [{ id: `temp-choice-${Date.now()}`, name: '', text: '', targetLabelId: '' }],
                };
                break;
        }

        setSteps((prevSteps) => {
            const insertionIndex = (selectedStepIndex !== null && selectedStepIndex !== -1)
                ? selectedStepIndex + 1
                : prevSteps.length;

            const updatedSteps = [
                ...prevSteps.slice(0, insertionIndex),
                tempStep,
                ...prevSteps.slice(insertionIndex)
            ];

            setSelectedStepIndex(insertionIndex);
            return updatedSteps;
        });
    };
    const deleteStep = (index: number) => {
        const newSteps = steps.filter((_, i) => i !== index);
        setSteps(newSteps);
        if (selectedStepIndex >= newSteps.length) {
            setSelectedStepIndex(newSteps.length - 1);
        }
    }
    const renderStepForm = () => {
        if (!currentStep) return <div>Выберите шаг</div>;

        const labelNames = labels.map(lab => lab.name);
        const formProps = {
            control,
            errors,
            register,
            setValue,
            characterOptions: characterOptions,
            labelOptions: labelNames,
            novelId:novelId,
        };

        switch (currentStep.type) {
            case 'hide_character':
                return <HideStepForm {...formProps} />;
            case 'jump':
                return <JumpStepForm {...formProps} />;
            case 'show_character':
                return <ShowStepForm {...formProps} />;
            case 'show_background':
                return <BackgroundStepForm {...formProps} />;
            case 'replica':
                return <ReplicaStepForm {...formProps} />;
            case 'menu':
                return <ChoiceStepForm {...formProps} />;
            default:
                return null;
        }
    };

    const changeLabel = (labelId) => {
        setSelectedLabelId(labelId);
    }

    const createLabel = async (e) => {
        e.preventDefault();
        try {
            const {data: newLabel} = await api.post<Label>(`/novels/${novelId}/labels`, {
                name: labelName || 'Новая сцена'
            })
            setLabels([...labels, newLabel]);
            setSelectedLabelId(newLabel.id);
        } catch (error) {
            console.error(error);
            alert('Не удалось создать сцену. Попробуйте еще раз.');
        } finally {
            setIsLabelOpen(false);
        }
    }

    const deleteLabel = async (id: string) => {
        try {
            await api.delete<Label>(`novels/${novelId}/labels/${id}`, {
                data: {
                    labelId: id,
                    novelId: novelId,
                }
            });
            const newLabels = labels.filter(lab => lab.id !== id);
            setLabels(newLabels);
            if (selectedLabelId === id) {
                setSelectedLabelId(newLabels[0].id);
            }
        } catch (error) {
            console.log(error);
        }
    }

    const patchLabel = async (label:Label) => {
        try {
            await api.patch<Label>(`/novels/${novelId}/labels/${label.id}`, {
                novelId: '0',
                labelId: label.id,
                name:label.name,
            })
            setLabels((prevLabels) =>
                prevLabels.map((lab) =>
                    lab.id === label.id ? { ...label, name: label.name } : lab
                )
            );
        } catch (error) {
            console.error(error);
        }
    }

    return (
        <div className={css({
            bg: '#775D68',
            minHeight: '100vh',
            color: 'text',
        })}>
            <Header active="editor"/>
            <main className={css({
                pt: '90px',
                pb: '0px',
                px: '0px',
            })}>
                <div className={css({
                    minHeight: '100vh',
                    background: '#775D68',
                    display: 'flex',
                    flexDirection: 'column',
                    width: '100%',
                    gap: '0px',
                })}>
                    <div className={css({
                        display: 'flex',
                        minHeight: '0',
                        flex: 1,
                        gap: '20px',
                        width: '100%',
                    })}>
                        <div className={css({
                            display: 'flex',
                            flexDirection: 'column',
                            width: '100%',
                            gap: '0px',
                        })}>
                            <EditorHeader active="editor"/>
                            <div className={css({
                                backgroundColor: 'white',
                                color: 'black',
                                width: '100%',
                                height: '100%',
                                paddingTop: '20px',
                                display: 'flex',
                                flexDirection: 'row',
                                gap: '20px',
                                flex: 4,
                            })}>
                                <div className={css({
                                    backgroundColor: '#DFC6D1',
                                    borderRadius: '5px',
                                    flex: 1,
                                    display: 'flex',
                                    flexDirection: 'column',
                                    alignItems: 'center',
                                })}>
                                    <div className={css({
                                        fontWeight: 'bold',
                                        fontSize: '20px',
                                        marginTop: '15px',
                                    })}>
                                        Сцены
                                    </div>
                                    <div className={css({
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: '10px',
                                        width: '100%',
                                        alignItems: 'center',
                                        marginTop: '20px',
                                    })}>
                                        {labels.map(label => (
                                            <LabelItem
                                                key={label.id}
                                                changeLabel={changeLabel}
                                                label={label}
                                                selectedId={selectedLabelId}
                                                onDelete={deleteLabel}
                                                onPatch={patchLabel}
                                                labelName={labelName}
                                                setLabelName={setLabelName}
                                            >
                                            </LabelItem>
                                        ))}
                                    </div>
                                    <button
                                        onClick={() => setIsLabelOpen(true)}
                                        className={css({
                                            bg: 'white',
                                            width: '80%',
                                            height: '5%',
                                            padding: '10px',
                                            margin: '20px auto',
                                            _hover: {
                                                bg: '#705661', color: 'white',
                                                boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                            },
                                            borderRadius: '12px',
                                            fontWeight: 'medium',
                                        })}>
                                        + Добавить сцену
                                    </button>
                                </div>
                                <div className={css({
                                    backgroundColor: 'white',
                                    color: 'black',
                                    display: 'flex',
                                    flexDirection: 'column',
                                    gap: '20px',
                                    flex: 4,
                                })}>
                                    <Preview steps={steps} selectedStepIndex={selectedStepIndex} control={control}></Preview>
                                    <BlockPanel
                                        steps={steps}
                                        selectedStepIndex={selectedStepIndex}
                                        onSelectStep={setSelectedStepIndex}
                                        onAddClick={() => setIsOpen(true)}
                                        onDeleteStep={deleteStep}
                                    />
                                </div>
                            </div>
                            <Modal active={isLabelOpen} setActive={setIsLabelOpen}>
                                <div className={css({
                                    display: 'flex',
                                    flexDirection: 'column',
                                    gap: '20px',
                                })}>

                                    <form onSubmit={createLabel} className={css({
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: '20px',
                                    })}>
                                        <div className={css({
                                            display: "flex",
                                            flexDirection: "column",
                                            gap: '10px',
                                            width: '300px',
                                            margin: '0 auto',
                                        })}>
                                            <label
                                                className={css({fontSize: '18px', textAlign: 'left'})}>Название</label>
                                            <input value={labelName}
                                                   onChange={(e) => setLabelName(e.target.value)}
                                                   required
                                                   className={css({
                                                       width: '100%',
                                                       padding: '10px',
                                                       borderRadius: '8px',
                                                       backgroundColor: 'white',
                                                       border: '1px solid black'
                                                   })}
                                            />
                                        </div>
                                        <button type="submit" className={css({
                                            alignSelf: 'flex-start',
                                            padding: '10px 20px',
                                            borderRadius: '8px',
                                            border: 'none',
                                            backgroundColor: '#705661',
                                            color: 'white',
                                            fontWeight: 'bold',
                                            margin: '0 auto',
                                            width: '300px',
                                            _hover: {bg: '#A87383'},
                                        })}>
                                            Создать
                                        </button>
                                    </form>
                                </div>
                            </Modal>
                            <Modal active={isOpen} setActive={setIsOpen}>
                                <div className={css({
                                    display: 'flex',
                                    flexDirection: 'column',
                                    gap: '20px',
                                })}>
                                    <div className={css({
                                        fontSize: '18px',
                                        fontWeight: 'bold',
                                    })}>
                                        Выберите тип блока
                                    </div>
                                    {Object.values(StepType).map((type) => {
                                        return (
                                            <button
                                                key={type}
                                                onClick={() => {
                                                    addStep(type);
                                                    setIsOpen(false);
                                                }}
                                                className={css({
                                                    padding: '10px',
                                                    borderRadius: '8px',
                                                    border: '1px solid #ccc',
                                                    backgroundColor: '#F8EDEB',
                                                    _hover: {bg: '#DFC6D1'},
                                                })}
                                            >
                                                {stepDisplayNames[type]}
                                            </button>
                                        );
                                    })}</div>
                            </Modal>
                        </div>
                        <div className={css({
                            backgroundColor: '#DFC6D1',
                            color: 'black',
                            flex: 1,
                            minWidth: '400px',
                            borderRadius: '12px',
                        })}>
                            {currentStep ? (
                                <form onSubmit={handleSubmit(onSave)} className={css({
                                    padding: '20px',
                                    display: 'flex',
                                    flexDirection: 'column',
                                    gap: '20px'
                                })}>
                                    <h2 className={css({
                                        fontSize: '20px',
                                        fontWeight: 'bold',
                                        borderBottom: '2px solid #705661'
                                    })}>
                                        {stepDisplayNames[currentStep.type as StepType]}
                                    </h2>
                                    {renderStepForm()}
                                    <button
                                        type="submit"
                                        className={css({
                                            alignSelf: 'flex-start',
                                            padding: '10px 20px',
                                            borderRadius: '8px',
                                            border: 'none',
                                            backgroundColor: '#705661',
                                            color: 'white',
                                            fontWeight: 'bold',
                                            margin: '0 auto',
                                            width: '300px',
                                            _hover: {bg: '#A87383'},
                                        })}
                                    >
                                        Сохранить
                                    </button>
                                </form>
                            ) : (
                                <div className={css({padding: '20px'})}>Выберите шаг для редактирования</div>
                            )}
                        </div>
                    </div>
                </div>
            </main>
        </div>
    )
}

