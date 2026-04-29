import Header from "../shared/ui/Header.tsx";
import {css} from '../../styled-system/css'
import {useEffect, useMemo, useRef, useState} from "react";
import EditorHeader from "../shared/ui/EditorHeader.tsx";
import Preview from "../shared/ui/Preview.tsx";
import BlockPanel from "../shared/ui/BlockPanel.tsx";
import Selector from "../shared/ui/Selector.tsx";
import {Controller, useFieldArray, useForm, useWatch} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {z} from "zod";
import Modal from "../shared/ui/Modal.tsx";
import api from "../api.tsx";
import {labelsApi, stepsApi, charactersApi, imagesApi} from "../shared/api/client";
import type {LabelResponse, StepResponse, CharacterResponse} from "../shared/api/types";
import {useParams} from "react-router-dom";
import {vstack, hstack} from '../../styled-system/patterns';
import {LabelItem} from "../shared/ui/LabelItem.tsx";
import type {Character} from "../shared/ui/AssetsContainer.tsx";
import {setLastNovelId} from "../shared/lib/novelSession.ts";

const DESIGN_RESOLUTION = { width: 1920, height: 1080 };

export const formatTransformForBackend = (transform: any) => {
    if (!transform) return null;

    return {
        ...transform,
        x: Number((transform.x / 100).toFixed(4)),
        y: Number((transform.y / 100).toFixed(4)),
        width: Math.round((transform.width / 100) * DESIGN_RESOLUTION.width),
        height: Math.round((transform.height / 100) * DESIGN_RESOLUTION.height),
        scale: transform.scale ?? 1,
        rotation: transform.rotation ?? 0,
        zIndex: Math.round(transform.zIndex ?? 0)
    };
};

export const formatTransformForFrontend = (backendTransform: any) => {
    if (!backendTransform) return null;

    return {
        ...backendTransform,
        x: Number((backendTransform.x * 100).toFixed(2)),
        y: Number((backendTransform.y * 100).toFixed(2)),
        width: Number(((backendTransform.width / DESIGN_RESOLUTION.width) * 100).toFixed(2)),
        height: Number(((backendTransform.height / DESIGN_RESOLUTION.height) * 100).toFixed(2)),
        scale: backendTransform.scale ?? 1,
        rotation: backendTransform.rotation ?? 0,
        zIndex: backendTransform.zIndex ?? 1
    };
};

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
const defaultTransform = {
    x: 40,
    y: 30,
    width: 25,
    height: 70,
    scale: 1,
    rotation: 0,
    zIndex: 20,
};

const defaultBackgroundTransform = {
    x: 0,
    y: 0,
    width: 100,
    height: 100,
    scale: 1,
    rotation: 0,
    zIndex: 1,
};

const defaultSceneState = {
    background: null,
    characters: [],
};

const baseStepSchema = z.object({
    id: z.string().nullable().optional(),
    state: sceneStateSchema.optional(),
});

const hideStepSchema = baseStepSchema.extend({
    type: z.literal('hide_character'),
    characterId: z.string().min(1, 'Выберите персонажа'),
});

const jumpStepSchema = z.object({
    id: z.string().nullable().optional(),
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

const choiceStepSchema = z.object({
    id: z.string().nullable().optional(),
    type: z.literal('menu'),
    menuRequest: z.object({
        id: z.string().optional(),
        choices: z.array(z.object({
            id: z.string().optional(),
            name: z.string().optional(),
            text: z.string().min(1, 'Введите текст выбора'),
            targetLabelId: z.string().min(1, 'Выберите сцену'),
        })).min(1, 'Добавьте хотя бы один вариант'),
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
    id: string,
    name: string,
    states: {
        id: string;
        name: string;
    }[],
}
type SelectorOption = {
    value: string;
    label: string;
};
type StepFormProps = {
    control: any;
    errors: any;
    register: any;
    characterOptions: CharacterOption[];
    labelOptions: SelectorOption[];
    setValue: any;
    novelId: string;
};

const normalizeIncomingStep = (step: any): Step => {
    // Преобразуем новые типы API в старую структуру для формы
    if (step.type === 'show_menu') {
        return {
            ...step,
            id: step.id || '',
            type: 'menu',
            menuRequest: {
                id: step.id || `temp-menu-${step.id}`,
                choices: step.menu?.choices?.map((c: any) => ({
                    text: c.text || '',
                    targetLabelId: c.transition?.targetLabelId || '',
                })) || [],
            },
        };
    }
    if (step.type === 'show_replica') {
        return {
            ...step,
            type: 'replica',
            characterId: step.replica?.speakerId || '',
            text: step.replica?.text || '',
        };
    }
    if (step.type === 'jump') {
        return {
            ...step,
            id: step.id || '',
            type: 'jump',
            targetId: step.transition?.targetLabelId || step.targetLabelId || '',
            targetLabelId: step.transition?.targetLabelId || step.targetLabelId || '',
        };
    }
    if (step?.type === 'show_background') {
        return {
            ...step,
            imageId: step.backgroundObject?.image?.id ?? step.imageId ?? '',
            transform: step.transform || defaultBackgroundTransform,
        }
    }
    if (step?.type === 'show_replica') {
        return {
            ...step,
            type: 'replica',
            characterId: step.replica?.speakerId ?? '',
            text: step.replica?.text ?? '',
        };
    }
    return step as Step;
};

function HideStepForm({control, errors, characterOptions}: StepFormProps) {
    const options = characterOptions.map(ch => ({
        value: ch.id,
        label: ch.name,
    }));
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

function ShowStepForm({control, errors, characterOptions, setValue}: StepFormProps) {
    const selectedCharacterId = useWatch({control, name: 'characterId'});
    const previousCharacterIdRef = useRef<string | undefined>();
    const selectedCharacter = characterOptions.find(ch => ch.id === selectedCharacterId);
    const options = characterOptions.map(ch => ({
        value: ch.id,
        label: ch.name
    }));
    const stateOptions = useMemo(() => {
        if (!selectedCharacter || !Array.isArray(selectedCharacter.states)) {
            return [];
        }

        return selectedCharacter.states.map(state => {
            // Если state — это объект {id, name}, берем name. Если строка — берем её саму.
            const label = typeof state === 'object' && state !== null ? (state.name || state.id) : state;
            const value = typeof state === 'object' && state !== null ? (state.id || state.name) : state;

            return {
                value: String(value),
                label: String(label)
            };
        });
    }, [selectedCharacter]);
    console.log(characterOptions)

    useEffect(() => {
        if (previousCharacterIdRef.current !== undefined && previousCharacterIdRef.current !== selectedCharacterId) {
            setValue('characterStateId', '');
        }

        previousCharacterIdRef.current = selectedCharacterId;
    }, [selectedCharacterId, setValue]);

    console.log('Character Options:', options);
    console.log('State Options:', stateOptions);
    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '12px'})}>
            <div className={css({display: 'flex', flexDirection: 'column', gap: '8px'})}>
                <Controller
                    control={control}
                    name="characterId"
                    render={({field}) =>
                        <Selector
                            title="Персонаж"
                            options={options}
                            {...field}
                            value={field.value ?? ''}
                        />}
                />
                <Controller
                    control={control}
                    name="characterStateId"
                    render={({field}) => (
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
                    <CompactInput label="X" name="transform.x" control={control}/>
                    <CompactInput label="Y" name="transform.y" control={control}/>

                    <CompactInput label="W" name="transform.width" control={control}/>
                    <CompactInput label="H" name="transform.height" control={control}/>

                    <CompactInput label="Scale" name="transform.scale" control={control} step="0.1"/>
                    <CompactInput label="Rot°" name="transform.rotation" control={control}/>

                    <div className={css({gridColumn: 'span 2'})}>
                        <CompactInput label="Z-Index" name="transform.zIndex" control={control}/>
                    </div>
                </div>
            </div>

            {(errors.characterId || errors.characterStateId) && (
                <p className={css({color: 'red', fontSize: '12px'})}>Заполните обязательные поля</p>
            )}
        </div>
    );
}

function CompactInput({label, name, control, step = "1"}: any) {
    return (
        <Controller
            name={name}
            control={control}
            render={({field}) => (
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
                            ':focus': {borderColor: '#007bff'}
                        })}
                        {...field}
                        value={field.value ?? 0}
                        onChange={(e) => field.onChange(Number(e.target.value))}
                    />
                </div>
            )}
        />
    );
}

export const getImageDimensions = (file: File): Promise<{ width: number; height: number }> => {
    return new Promise((resolve) => {
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = new Image();
            img.onload = () => resolve({width: img.width, height: img.height});
            img.src = e.target?.result as string;
        };
        reader.readAsDataURL(file);
    });
};

function BackgroundStepForm({control, errors, setValue, novelId}: StepFormProps) {
    const [isUploading, setIsUploading] = useState(false);
    const backgroundImageId = useWatch({
        control,
        name: 'background.imageId'
    });
    const stepImageId = useWatch({
        control,
        name: 'imageId'
    });
    const currentImageId = backgroundImageId || stepImageId;

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
            const {imageId, uploadUrl, viewUrl} = response.data;

            console.log('imageId:', imageId);

            if (uploadUrl) {
                await api.put(uploadUrl, file, {
                    headers: {'Content-Type': file.type}
                });
            }

            if (imageId) {
                setValue('imageId', imageId);
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
        <div className={css({display: 'flex', flexDirection: 'column', gap: '16px'})}>
            <div className={css({display: 'flex', flexDirection: 'column', gap: '8px'})}>
                <label className={css({fontWeight: 'bold', fontSize: '14px'})}>Изображение фона</label>

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
                        '&:hover': {backgroundColor: '#000'}
                    })}>
                        {isUploading ? 'Загрузка...' : 'Выбрать и загрузить'}
                        <input
                            type="file"
                            accept="image/*"
                            onChange={handleFileUpload}
                            disabled={isUploading}
                            className={css({display: 'none'})}
                        />
                    </label>
                </div>
                {errors.background?.imageId && (
                    <p className={css({color: 'red', fontSize: '12px'})}>
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
                <div className={css({
                    fontSize: '11px',
                    fontWeight: 'bold',
                    marginBottom: '10px',
                    color: '#888',
                    textTransform: 'uppercase'
                })}>
                    Позиция фона
                </div>

                <div className={css({display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '8px 12px'})}>
                    <CompactInput label="X" name="background.transform.x" control={control}/>
                    <CompactInput label="Y" name="background.transform.y" control={control}/>
                    <CompactInput label="W" name="background.transform.width" control={control}/>
                    <CompactInput label="H" name="background.transform.height" control={control}/>
                    <CompactInput label="Scale" name="background.transform.scale" control={control} step="0.1"/>
                    <CompactInput label="Rot°" name="background.transform.rotation" control={control}/>
                    <div className={css({gridColumn: 'span 2'})}>
                        <CompactInput label="Z-Index" name="background.transform.zIndex" control={control}/>
                    </div>
                </div>
            </div>
        </div>
    );
}

function ReplicaStepForm({control, errors, register, characterOptions}: StepFormProps) {
    const characterSelectOptions = characterOptions.map((character) => ({
        value: character.id,
        label: character.name,
    }));
    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '16px'})}>
            <Controller
                control={control}
                name="characterId"
                render={({field}) => <Selector title="Персонаж" options={characterSelectOptions} {...field} />}
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
                <Controller
                    control={control}
                    name="text"
                    render={({field}) => (
                        <textarea
                            {...field}
                            value={field.value || ''}
                            className={css({
                                width: '300px',
                                minHeight: '120px',
                                padding: '12px',
                                borderRadius: '8px',
                                backgroundColor: 'white',
                                border: '1px solid black',
                            })}
                        />
                    )}
                />
            </div>
            {errors.text && <p className={css({color: 'red', fontSize: '13px'})}>{errors.text.message}</p>}
        </div>
    );
}

function ChoiceStepForm({control, labelOptions, register, errors}: StepFormProps) {
    const {fields, append, remove} = useFieldArray({
        control,
        name: "menuRequest.choices"
    });

    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '20px', width: '350px', margin: '0 auto'})}>
            <div className={css({display: 'flex', flexDirection: 'column', gap: '12px'})}>
                <div className={css({display: 'flex', justifyContent: 'space-between', alignItems: 'center'})}>
                    <label className={css({fontSize: '16px', fontWeight: 'bold'})}>Варианты ответов</label>
                    <button
                        type="button"
                        onClick={() =>
                            append({
                                id: '',
                                name: '',
                                text: '',
                                targetLabelId: '',
                            })
                        }
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
                        <input
                            type="hidden"
                            {...register(`menuRequest.choices.${index}.id` as const)}
                        />
                        {/* Ошибка валидации для текста */}
                        {errors?.menuRequest?.choices?.[index]?.text && (
                            <p className={css({color: 'red'})}>
                                {errors.menuRequest.choices[index].text.message}
                            </p>
                        )}
                        {/* Кнопка удаления варианта */}
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

                        <div className={css({display: 'flex', flexDirection: 'column', gap: '8px'})}>
                            <label className={css({fontSize: '12px', fontWeight: 'bold', color: '#666'})}>Текст
                                кнопки</label>
                            <input
                                {...register(`menuRequest.choices.${index}.text` as const)}
                                className={compactInputStyle}
                                placeholder="Текст ответа"
                            />
                        </div>

                        <div className={css({display: 'flex', flexDirection: 'column', gap: '8px'})}>
                            <label className={css({fontSize: '12px', color: '#666'})}>Переход на сцену</label>
                            <Controller
                                control={control}
                                name={`menuRequest.choices.${index}.targetLabelId` as const}
                                render={({field: selectField}) => (
                                    <Selector
                                        title="Переход на сцену"
                                        options={labelOptions}
                                        {...selectField}
                                    />
                                )}
                            />
                            {/* Ошибка валидации для targetLabelId */}
                            {errors?.menuRequest?.choices?.[index]?.targetLabelId && (
                                <p className={css({color: 'red'})}>
                                    {errors.menuRequest.choices[index].targetLabelId.message}
                                </p>
                            )}
                        </div>
                    </div>
                ))}

                {fields.length === 0 && (
                    <p className={css({fontSize: '12px', color: '#888', textAlign: 'center', padding: '12px 0'})}>
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
    const {novelId} = useParams<{ novelId: string }>();
    const [isOpen, setIsOpen] = useState(false);
    const [steps, setSteps] = useState<Step[]>([]);
    const [labels, setLabels] = useState<Label[]>([]);
    const [imageUrl, setImageUrl] = useState('');
    const [selectedLabelId, setSelectedLabelId] = useState<string | null>(labels[0]?.id ?? null);
    const [selectedStepIndex, setSelectedStepIndex] = useState(0);
    const [selectedId, setSelectedId] = useState<string | null>(steps[0]?.id ?? null);
    const [newStepData, setNewStepData] = useState<any>(null); // Для нового step до сохранения
    const currentStep = newStepData || steps[selectedStepIndex]; // Используем newStepData если есть
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
        watch,
        formState: {errors},
    } = useForm<Step>({
        resolver: zodResolver(stepSchema),
        mode: 'onChange',
        defaultValues: {
            state: defaultSceneState,
            characterId: '',
            text: currentStep?.text ?? '',
            characterStateId: '',
            menuRequest: {
                choices: [],
            },
            background: {
                imageId: '',
                transform: defaultBackgroundTransform
            },
            characters: []
        }
    });

    useEffect(() => {
        if (novelId) {
            setLastNovelId(novelId);
        }
    }, [novelId]);

    useEffect(() => {
        const fetchCharacterNames = async () => {
            try {
                const {data} = await charactersApi.getAll(novelId);
                setCharacterOptions(data.map(ch => ({
                    id: ch.id,
                    name: ch.name,
                    nameColor:ch.nameColor || '#ffffff',
                    states: ch.characterStates.map(st => ({
                        id: st.id,
                        name: st.name,
                        imageId: st.image?.id || '',
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
            const dataForForm = {
                ...currentStep,
                state: currentStep.state ?? defaultSceneState,
                characterId: currentStep.characterId ?? '',
                text: currentStep.text ?? '',
                characterStateId: currentStep.characterStateId ?? '',
                background: {
                    imageId: currentStep.background?.imageId || currentStep.imageId || '',
                    transform: currentStep.background?.transform || currentStep.transform || defaultBackgroundTransform
                },
                transform: currentStep.transform || (currentStep.type === 'show_character' ? defaultTransform : defaultBackgroundTransform)
            };
            reset(dataForForm);
        }
    }, [selectedStepIndex, currentStep, reset]);
    useEffect(() => {
        const loadStepData = async () => {
            if (!selectedId) return;

            try {
                setLoading(true);
                // Этот endpoint не используется - данные уже есть в steps
                // const {data: rawStep} = await stepsApi.getById(novelId, selectedLabelId, selectedId);
                // const data = normalizeIncomingStep(rawStep);

                // Используем данные из локального состояния
                const currentStep = steps.find(s => s.id === selectedId);
                if (!currentStep) return;

                const data = normalizeIncomingStep(currentStep);
                const baseData = ({
                    id: data.id,
                    type: data.type,
                });
                switch (data.type) {
                    case 'show_character':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            characterId: data.characterObject.id ?? '',
                            characterStateId: data.state.id ?? '',
                            transform: formatTransformForFrontend(data.characterObject.transform) ?? defaultTransform,
                        });
                        break;

                    case 'hide_character':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            characterId: data.characterId ?? '',
                        });
                        break;

                    case 'show_background':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            imageId: data.backgroundObject?.image?.id ?? data.imageId ?? '',
                            background: {
                                imageId: data.backgroundObject?.image?.id ?? data.imageId ?? '',
                                transform: formatTransformForFrontend(data.backgroundObject?.transform) ?? defaultBackgroundTransform,
                            },
                            transform: formatTransformForFrontend(data.transform) ?? defaultBackgroundTransform,
                        });
                        break;

                    case 'jump':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            targetId: data.targetId,
                        });
                        break;

                    case 'replica':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            characterId: data.characterId ?? '',
                            text: data.text ?? '',
                        });
                        break;

                    case 'menu':
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                            menuRequest: data.menuRequest ?? {
                                id: `temp-menu-${data.id}`,
                                choices: [],
                            },
                        });
                        break;

                    default:
                        reset({
                            ...baseData,
                            state: data.state ?? defaultSceneState,
                        });
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
                const {data} = await labelsApi.getAll(novelId);
                setLabels(data);

                if (data.length > 0) {
                    setSelectedLabelId(data[0].id);
                }
            } catch (error) {
                console.error(error);
                alert('Не удалось загрузить сцены');
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
                setSelectedId(null);
                return;
            }
            try {
                setLoading(true);
                const {data} = await stepsApi.getAll(novelId, selectedLabelId);
                const normalizedSteps = data.map((step) => normalizeIncomingStep(step));
                setSteps(normalizedSteps);

                if (normalizedSteps.length > 0) {
                    setSelectedId(normalizedSteps[0].id);
                } else {
                    setSelectedId(null);
                }
            } catch (error) {
                console.error(error);
                alert('Не удалось загрузить шаги');
            } finally {
                setLoading(false);
            }
        };
        console.log(steps)

        fetchSteps();
    }, [selectedLabelId, novelId]);
    const onSave = async (data: any) => {
        const finalState = {
            ...defaultSceneState,
            ...(data.state || {}),
            characters: data.state?.characters || defaultSceneState.characters,
        };

        const {type, ...restData} = data;
        const finalData = {
            type,
            ...restData,
            state: finalState
        };

        if (data.type === 'show_background' && data.background) {
            finalData.transform = data.background.transform;
            finalData.imageId = data.background.imageId;

            finalState.background = data.background;
        }

        if (data.type === 'show_character' && data.characterId) {
            const newChar = {
                characterId: data.characterId,
                characterStateId: data.characterStateId || null,
                transform: data.transform || {x: 40, y: 30, width: 25, height: 65, scale: 1, rotation: 0, zIndex: 20}
            };

            finalState.characters = [
                ...(finalState.characters || []).filter((ch: any) => ch.characterId !== data.characterId),
                newChar
            ];
        }

        if (data.type === 'hide_character' && data.characterId) {
            finalState.characters = (finalState.characters || [])
                .filter((ch: any) => ch.characterId !== data.characterId);
        }

        if (finalData.type === 'menu' && finalData.menuRequest?.choices) {
            finalData.menuRequest = {
                ...finalData.menuRequest,
                choices: finalData.menuRequest.choices.map((choice: any) => ({
                    ...choice,
                    id: choice.id || '',
                    name: choice.name?.trim() || choice.text,
                })),
            };
        }

        // Преобразуем данные под новое API
        if (data.type === 'jump' && data.targetId) {
            finalData.type = 'jump';
            finalData.targetLabelId = data.targetId;
        }

            // Menu step: menuRequest.choices -> choices
            if (data.type === 'menu' && data.menuRequest?.choices) {
                finalData.type = 'show_menu';
                finalData.choices = data.menuRequest.choices.map((choice: any) => ({
                    text: choice.text || choice.name || '',
                    targetLabelId: choice.targetLabelId || '',
                }));
            }

            // Replica step
            if (data.type === 'replica') {
                finalData.type = 'replica';
                finalData.speakerId = data.characterId || '';
                finalData.text = data.text || '';
            }

            // Show character step
            if (data.type === 'show_character') {
                finalData.characterId = data.characterId;
                finalData.characterStateId = data.characterStateId;
                finalData.transform = formatTransformForBackend(
                    data.transform || data.background?.transform || defaultBackgroundTransform
                );
            }

            // Hide character step
            if (data.type === 'hide_character') {
                finalData.characterId = data.characterId;
            }

            // Background step
            if (data.type === 'show_background') {
                finalData.imageId = data.imageId;
                finalData.transform = formatTransformForBackend(
                    data.transform || data.background?.transform || defaultBackgroundTransform
                );
            }

            finalData.state = finalState
            console.log(finalData);
            try {
                let savedStep: Step;

            console.log(data);
            if (data.id) {
                const { data: updated } = await stepsApi.patch(novelId, selectedLabelId, data.id, finalData);
                savedStep = normalizeIncomingStep(updated);
                setSteps(prev => prev.map(s => s.id === savedStep.id ? savedStep : s));
                setSelectedId(savedStep.id);
            } else {
                const { data: newStep } = await stepsApi.create(novelId, selectedLabelId, finalData);
                savedStep = normalizeIncomingStep(newStep);
                setSteps(prev => [...prev, savedStep]);
                setNewStepData(null);
                setSelectedId(null);
                setSelectedStepIndex(-1);
            }

            alert('Шаг сохранён');
        } catch (error) {
            console.error(error);
            alert('Не удалось сохранить шаг');
        }
    };
    const addStep = (type: StepType) => {
        // Проверяем ограничения на jump и menu
        const hasJump = steps.some(s => s.type === 'jump');
        const hasMenu = steps.some(s => s.type === 'menu' || s.type === 'show_menu');

        if (type === 'jump' && hasJump) {
            alert('В сцене может быть только один шаг перехода (jump)');
            return;
        }

        if (type === 'menu' && hasMenu) {
            alert('В сцене может быть только один шаг выбора (menu)');
            return;
        }

        if (type === 'jump' && hasMenu) {
            alert('Нельзя добавить переход (jump), если уже есть выбор (menu)');
            return;
        }

        if (type === 'menu' && hasJump) {
            alert('Нельзя добавить выбор (menu), если уже есть переход (jump)');
            return;
        }

        let tempStep: any = {
            id: '',
            type: '',
            state: defaultSceneState,
        };

        switch (type) {
            case 'show_background':
                tempStep.background = {
                    imageId: '',
                    transform: {...defaultBackgroundTransform}
                };
                tempStep.type = 'show_background';
                tempStep.transform = {x: 0, y: 0, width: 100, height: 100, scale: 1, rotation: 0, zIndex: 0};
                break;

            case 'show_character':
                tempStep.type = 'show_character';
                tempStep.characterId = '';
                tempStep.characterStateId = '';
                tempStep.transform = {x: 50, y: 50, width: 25, height: 60, scale: 1, rotation: 0, zIndex: 10};
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
                    choices: [],
                };
                break;
        }

        // Сохраняем временный step для отображения формы
        setNewStepData(tempStep);
        setSelectedId(null);
        setSelectedStepIndex(-1);
        reset(tempStep);
    };
    const deleteStep = async (index: number) => {
        const stepToDelete = steps[index];
        if (!stepToDelete) {
            return;
        }

        try {
            await stepsApi.delete(novelId, selectedLabelId, stepToDelete.id);
            const newSteps = steps.filter((_, i) => i !== index);
            setSteps(newSteps);

            if (newSteps.length === 0) {
                setSelectedStepIndex(0);
                setSelectedId(null);
                return;
            }

            const nextIndex = Math.min(selectedStepIndex, newSteps.length - 1);
            setSelectedStepIndex(nextIndex);
            setSelectedId(newSteps[nextIndex].id);
        } catch (error) {
            console.error(error);
            alert('Не удалось удалить шаг');
        }
    };

    const moveStep = (fromIndex: number, toIndex: number) => {
        if (fromIndex === toIndex) return;

        const newSteps = [...steps];
        const [movedStep] = newSteps.splice(fromIndex, 1);
        newSteps.splice(toIndex, 0, movedStep);

        setSteps(newSteps);

        // Обновляем выбранный индекс
        if (selectedStepIndex === fromIndex) {
            setSelectedStepIndex(toIndex);
        } else if (selectedStepIndex === toIndex) {
            setSelectedStepIndex(fromIndex < toIndex ? toIndex - 1 : toIndex + 1);
        }
    };

    const handleSelectStep = (index: number) => {
        setSelectedStepIndex(index);
        setSelectedId(steps[index]?.id ?? null);
    };

    const renderStepForm = () => {
        if (!currentStep) return null;

        const labelOptions = labels.map(label => ({
            value: label.id,
            label: label.name
        }));

        const formProps = {
            register,
            control,
            errors,
            register,
            setValue,
            watch,
            labelOptions,
            characterOptions,
            novelId,
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
            const {data: newLabel} = await labelsApi.create(novelId, {
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
            await labelsApi.delete(novelId, id);
            const newLabels = labels.filter(lab => lab.id !== id);
            setLabels(newLabels);
            if (selectedLabelId === id) {
                if (newLabels.length > 0) {
                    setSelectedLabelId(newLabels[0].id);
                } else {
                    setSelectedLabelId(null);
                    setSteps([]);
                }
            }
        } catch (error) {
            console.error(error);
            alert('Не удалось удалить сцену');
        }
    }

    const patchLabel = async (label: Label) => {
        try {
            await labelsApi.patch(novelId, label.id, {
                name: label.name,
            })
            setLabels((prevLabels) =>
                prevLabels.map((lab) =>
                    lab.id === label.id ? {...label, name: label.name} : lab
                )
            );
        } catch (error) {
            console.error(error);
            alert('Не удалось обновить сцену');
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
                                            minHeight: '44px',
                                            padding: '10px 14px',
                                            margin: '20px auto',
                                            _hover: {
                                                bg: '#705661', color: 'white',
                                                boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                            },
                                            borderRadius: '12px',
                                            fontWeight: 'medium',
                                            flexShrink: 0,
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
                                    <Preview
                                        labelId={selectedLabelId}
                                        stepId={selectedId}
                                        control={control}
                                        novelId={novelId}
                                        characterOptions={characterOptions}
                                    ></Preview>
                                    <BlockPanel
                                        steps={steps}
                                        selectedStepIndex={selectedStepIndex}
                                        onSelectStep={handleSelectStep}
                                        onAddClick={() => setIsOpen(true)}
                                        onDeleteStep={deleteStep}
                                        onMoveStep={moveStep}
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
                                                className={css({
                                                    fontSize: '18px',
                                                    textAlign: 'left'
                                                })}>Название</label>
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
                                <form
                                    onSubmit={handleSubmit(onSave, (errors) => console.log('Форма невалидна', errors))}
                                    className={css({
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
