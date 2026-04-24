import Header from "../shared/ui/Header.tsx";
import {css} from '../../styled-system/css'
import {useEffect, useState} from "react";
import EditorHeader from "../shared/ui/EditorHeader.tsx";
import Preview from "../shared/ui/Preview.tsx";
import BlockPanel from "../shared/ui/BlockPanel.tsx";
import Selector from "../shared/ui/Selector.tsx";
import {Controller, useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {z} from "zod";
import Modal from "../shared/ui/Modal.tsx";
import api from "../api.tsx";
import {useParams} from "react-router-dom";
import {vstack, hstack} from '../../styled-system/patterns';
import {LabelItem} from "../shared/ui/LabelItem.tsx";

export enum StepType {
    BACKGROUND = 'background',
    SHOW = 'show',
    HIDE = 'hide',
    REPLICA = 'replica',
    JUMP = 'jump',
    CHOICE = 'choice',
}

export const stepDisplayNames: Record<StepType, string> = {
    [StepType.BACKGROUND]: 'Фон',
    [StepType.SHOW]: 'Появление',
    [StepType.HIDE]: 'Исчезновение',
    [StepType.REPLICA]: 'Реплика',
    [StepType.JUMP]: 'Переход',
    [StepType.CHOICE]: 'Выбор',
};
const baseStepSchema = z.object({
    id: z.string().min(1),
});
const transformSchema = z.object({
    x: z.number(),
    y: z.number(),
    width: z.number(),
    height: z.number(),
    scale: z.number(),
    rotation: z.number(),
    zIndex: z.number(),
});

const hideStepSchema = baseStepSchema.extend({
    type: z.literal('hide'),
    characterId: z.string().min(1, 'Выберите персонажа'),
});

const jumpStepSchema = baseStepSchema.extend({
    type: z.literal('jump'),
    targetId: z.string().min(1, 'Выберите следующую сцену'),
});

const showStepSchema = baseStepSchema.extend({
    type: z.literal('show'),
    characterId: z.string().min(1),
    characterStateId: z.string().min(1),
    transform: transformSchema,
});

const backgroundStepSchema = baseStepSchema.extend({
    type: z.literal('background'),
    imageId: z.string().min(1),
    transform: transformSchema,
});

const replicaStepSchema = baseStepSchema.extend({
    type: z.literal('replica'),
    characterId: z.string().min(1),
    text: z.string().min(1, 'Введите текст реплики'),
});

const choiceStepSchema = baseStepSchema.extend({
    type: z.literal('choice'),
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

const CHARACTERS = ['Анна', 'Мария', 'Борис'];
const SCENES = ['Парк', 'Улица', 'Дом'];
type StepFormProps = {
    control: any;
    errors: any;
    register: any;
};

function HideStepForm({control, errors}: StepFormProps) {
    return (
        <div>
            <Controller
                control={control}
                name="characterId"
                render={({field}) => <Selector title="Персонаж" options={CHARACTERS} {...field} />}
            />
            {errors.characterId && <p className={css({color: 'red'})}>{errors.characterId.message}</p>}
        </div>
    );
}

function JumpStepForm({control, errors}: StepFormProps) {
    return (
        <div>
            <Controller
                control={control}
                name="targetId"
                render={({field}) => <Selector title="Сцена" options={SCENES} {...field} />}
            />
            {errors.targetId && <p className={css({color: 'red'})}>{errors.targetId.message}</p>}
        </div>
    );
}

function ShowStepForm({control, errors}: StepFormProps) {
    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '16px'})}>
            <div>
                <Controller
                    control={control}
                    name="characterId"
                    render={({field}) => <Selector title="Персонаж" options={CHARACTERS} {...field} />}
                />
                {errors.characterId &&
                    <p className={css({color: 'red', fontSize: '13px'})}>{errors.characterId.message}</p>}
            </div>
            <div>
                <label className={css({fontWeight: 'bold', display: 'block'})}>Состояние персонажа</label>
                <Controller
                    control={control}
                    name="characterStateId"
                    render={({field}) => <Selector title="Состояние" options={['normal', 'happy', 'sad']} {...field} />}
                />
                {errors.characterStateId &&
                    <p className={css({color: 'red', fontSize: '13px'})}>{errors.characterStateId.message}</p>}
            </div>
            {/* Transform можно вынести в отдельный компонент позже */}
            <div className={css({fontSize: '13px', color: '#666'})}>
                Transform (x, y, scale и т.д.) — можно добавить позже
            </div>
        </div>
    );
}

function BackgroundStepForm({control, errors}: StepFormProps) {
    return (
        <div>
            <Controller
                control={control}
                name="imageId"
                render={({field}) => <Selector title="Фон" options={['bg_park', 'bg_street', 'bg_room']} {...field} />}
            />
            {errors.imageId && <p className={css({color: 'red', fontSize: '13px'})}>{errors.imageId.message}</p>}
            <div className={css({fontSize: '13px', color: '#666', marginTop: '8px'})}>
                Transform (x, y, scale и т.д.) — можно добавить позже
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

function ChoiceStepForm({control, errors}: StepFormProps) {
    return (
        <div className={css({display: 'flex', flexDirection: 'column', gap: '16px'})}>
            <div className={css({
                display: "flex",
                flexDirection: "column",
                gap: '10px',
                width: '300px',
                margin: '0 auto',
            })}>
                <label className={css({fontSize: '18px', textAlign: 'left'})}>Название</label>
                <input
                    {...control.register('name')}
                    className={css({
                        width: '100%',
                        padding: '10px',
                        borderRadius: '8px',
                        backgroundColor: 'white',
                        border: '1px solid black'
                    })}
                />
            </div>

            <div className={css({
                display: "flex",
                flexDirection: "column",
                gap: '10px',
                width: '300px',
                margin: '0 auto',
            })}>
                <label className={css({fontSize: '18px', textAlign: 'left'})}>Текст</label>
                <input
                    {...control.register('text')}
                    className={css({
                        width: '100%',
                        padding: '10px',
                        borderRadius: '8px',
                        backgroundColor: 'white',
                        border: '1px solid black'
                    })}
                />
            </div>

            <div className={css({fontSize: '13px', color: '#666', marginTop: '20px'})}>
                Выборы (menuRequest.choices) — будет отдельно с useFieldArray позже
            </div>
        </div>
    );
}

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
    const {
        register,
        handleSubmit,
        reset,
        control,
        formState: {errors},
    } = useForm<Step>({
        resolver: zodResolver(stepSchema),
        mode: 'onChange',
    });

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
        const fetchSteps = async () => {
            try {
                setLoading(true);
                const {data} = await api.get<Step[]>('novels/0/labels/0/steps');
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
    }, []);
    const onSave = (formData: Step) => {
        const newSteps = [...steps];
        newSteps[selectedStepIndex] = formData;
        setSteps(newSteps);
    };
    const addStep = async (type: StepType) => {
        let newStep;

        switch (type) {
            case 'hide':
                newStep = {
                    type: 'hide',
                    characterId: '',
                };
                break;

            case 'show':
                newStep = {
                    type: 'show',
                    characterId: '',
                    characterStateId: '',
                    transform: {x: 0, y: 0, width: 0, height: 0, scale: 1, rotation: 0, zIndex: 0},
                };
                break;

            case 'background':
                newStep = {
                    type: 'background',
                    imageId: '',
                    transform: {x: 0, y: 0, width: 0, height: 0, scale: 1, rotation: 0, zIndex: 0},
                };
                break;

            case 'replica':
                newStep = {
                    type: 'replica',
                    characterId: '',
                    text: '',
                };
                break;

            case 'jump':
                newStep = {
                    type: 'jump',
                    targetId: '',
                };
                break;

            case 'choice':
                newStep = {
                    type: 'choice',
                    name: '',
                    text: '',
                    menuRequest: {
                        id: `temp-menu-${Date.now()}`,
                        name: null,
                        text: null,
                        choices: [
                            {
                                id: `temp-choice-${Date.now()}`,
                                name: '',
                                text: '',
                                targetLabelId: '',
                            },
                        ],
                    },
                };
                break;
        }
        try {
            const { data: serverStep } = await api.post<Step>('/novels/0/labels/0/steps', newStep);
            setSteps((prevSteps) => {
                const insertionIndex = (selectedStepIndex !== null && selectedStepIndex !== -1)
                    ? selectedStepIndex + 1
                    : prevSteps.length;
                const updatedSteps = [
                    ...prevSteps.slice(0, insertionIndex),
                    serverStep,
                    ...prevSteps.slice(insertionIndex)
                ];
                setSelectedStepIndex(insertionIndex);

                return updatedSteps;
            });

        } catch (error) {
            console.error('Ошибка создания:', error);
            alert('Не удалось создать шаг');
        }
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

        const formProps = {control, errors, register};

        switch (currentStep.type) {
            case 'hide':
                return <HideStepForm {...formProps} />;
            case 'jump':
                return <JumpStepForm {...formProps} />;
            case 'show':
                return <ShowStepForm {...formProps} />;
            case 'background':
                return <BackgroundStepForm {...formProps} />;
            case 'replica':
                return <ReplicaStepForm {...formProps} />;
            case 'choice':
                return <ChoiceStepForm {...formProps} />;
            default:
                return null;
        }
    };

    const changeLabel = (labelId) => {
        setSelectedLabelId(labelId);
    }

    useEffect(() => {
        const fetchLabels = async () => {
            try {
                setLoading(true);
                const {data} = await api.get<Label[]>('/novels/0/labels');
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
    }, []);
    const createLabel = async (e) => {
        e.preventDefault();
        try {
            const {data: newLabel} = await api.post<Label>(`/novels/0/labels`, {
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
            await api.delete<Label>(`novels/0/labels/${id}`, {
                data: {
                    labelId: id,
                    novelId: '0',
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
            await api.patch<Label>(`/novels/0/labels/${label.id}`, {
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

    const changePreview = async() => {
        try {
            //тут будет больно
        } catch (error) {
            console.error(error);
        }
    };

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
                                    <Preview image="src\assets\img.png"></Preview>
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