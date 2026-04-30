import {
    ReactFlow,
    Background,
    Controls,
    Handle,
    MarkerType,
    Position,
    useEdgesState,
    useNodesState,
    type Connection,
    type Edge,
    type Node,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { css } from '../../../styled-system/css';
import { useCallback, useEffect, useMemo, useState } from 'react';
import api, { labelsApi, stepsApi } from '../api/client';
import type { LabelResponse, StepResponse } from '../api/types';
import previewImage from '../../assets/img.png';
import {formatTransformForFrontend} from "../../pages/Editor.tsx";

type Label = {
    id: string;
    name: string;
};

type StepChoice = {
    id?: string;
    name?: string;
    text?: string;
    targetLabelId?: string;
    transition?: {
        targetLabelId?: string;
    };
};

type Step = {
    id: string;
    type: string;
    targetId?: string;
    imageId?: string;
    state?: {
        background?: {
            imageId?: string;
        } | null;
    };
    menuRequest?: {
        id?: string;
        choices?: StepChoice[];
    };
    choices?: StepChoice[];
};

type InfiniteCanvasProps = {
    novelId: string;
};

type SceneNodeData = {
    label: string;
    previewUrl: string;
    isStart?: boolean;
    choices: Array<{
        handleId: string;
        text: string;
        stepId: string;
        choiceIndex: number;
    }>;
};

const findPreviewData = (steps: Step[]) => {
    const bgStep = steps.find(s => s.type === 'show_background');
    if (!bgStep?.backgroundObject) return null;

    return {
        url: bgStep.backgroundObject.image?.url,
        transform: bgStep.backgroundObject.transform
    };
};

type SceneNode = Node<SceneNodeData>;
type SceneEdgeData =
    | { kind: 'jump'; stepId: string; sourceLabelId: string }
    | { kind: 'choice'; stepId: string; choiceIndex: number; sourceLabelId: string };

type SceneEdge = Edge<SceneEdgeData>;

const JUMP_SOURCE_HANDLE_ID = 'jump-source';
const CHOICE_HANDLE_PREFIX = 'choice';

const buildChoiceHandleId = (stepId: string, choiceIndex: number): string =>
    `${CHOICE_HANDLE_PREFIX}:${stepId}:${choiceIndex}`;

const parseChoiceHandleId = (handleId: string): { stepId: string; choiceIndex: number } | null => {
    const [prefix, stepId, choiceIndexRaw] = handleId.split(':');
    if (prefix !== CHOICE_HANDLE_PREFIX || !stepId) {
        return null;
    }

    const choiceIndex = Number(choiceIndexRaw);
    if (Number.isNaN(choiceIndex)) {
        return null;
    }

    return { stepId, choiceIndex };
};

const SceneGraphNode = ({ data }: { data: SceneNodeData }) => {
    // Вычисляем стили превью (теперь с трансформацией)
    const previewStyles = useMemo(() => {
        if (!data.preview?.transform) return null;
        const t = formatTransformForFrontend(data.preview.transform);
        return {
            position: 'absolute' as const,
            left: `${t.x}%`,
            top: `${t.y}%`,
            width: `${t.width}%`,
            height: `${t.height}%`,
            transform: `scale(${t.scale}) rotate(${t.rotation}deg)`,
            zIndex: t.zIndex,
            objectFit: 'fill' as const
        };
    }, [data.preview]);

    return (
        <div className={css({
            width: '240px', // Чуть шире для красоты
            border: `2px solid ${data.isStart ? '#f59e0b' : '#775D68'}`,
            borderRadius: '12px',
            backgroundColor: 'white',
            overflow: 'hidden',
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            position: 'relative',
        })}>
            {/* ВХОД (Target) — общее для всех ребер, входящих в сцену */}
            <Handle
                type="target"
                position={Position.Left}
                style={{ background: '#775D68', width: 10, height: 10, left: -6 }}
            />

            {/* ПРЕВЬЮ ФОНА */}
            <div className={css({
                height: '120px',
                width: '100%',
                backgroundColor: '#000',
                position: 'relative',
                overflow: 'hidden',
                borderBottom: '1px solid #eee',
                textAlign: 'center',
            })}>
                {data.preview?.url ? (
                    <img
                        src={data.preview.url}
                        style={previewStyles || { width: '100%', height: '100%' }}
                        alt="Preview"
                    />
                ) : (
                    <div className={css({ color: '#444', fontSize: '10px', display:'flex', justifyContent:'center' })}>НЕТ ФОНА</div>
                )}
            </div>

            <div className={css({ padding: '8px', fontWeight: 'bold', textAlign: 'center' })}>
                {data.label}
            </div>

            {/* Входы для выбора (choices) */}
            {data.choices.length > 0 && (
                <div className={css({
                    display: 'flex', flexDirection: 'column', gap: '4px', padding: '0 8px 10px 8px',
                })}>
                    {data.choices.map((choice) => (
                        <div key={choice.handleId} className={css({
                            position: 'relative', padding: '4px 8px',
                            border: '1px solid #e9d5ff', borderRadius: '6px',
                            fontSize: '11px', backgroundColor: '#faf5ff',
                        })}>
                            {choice.text}
                            <Handle
                                type="source"
                                position={Position.Right}
                                id={choice.handleId}
                                style={{ background: '#6b21a8', right: -12 }}
                            />
                        </div>
                    ))}
                </div>
            )}

            {/* Общий выход для Jump (если нет выборов) */}
            {data.choices.length === 0 && (
                <Handle
                    type="source"
                    position={Position.Right}
                    id={JUMP_SOURCE_HANDLE_ID}
                    style={{ background: '#111', width: 8, height: 8 }}
                />
            )}
        </div>
    );
};

const extractChoiceTarget = (choice: StepChoice): string | null => {
    const directTarget = choice.targetLabelId?.trim();
    if (directTarget) {
        return directTarget;
    }

    const transitionTarget = choice.transition?.targetLabelId?.trim();
    return transitionTarget || null;
};

const extractStepChoices = (step: Step): StepChoice[] => {
    // Для show_menu из нового API
    if (step.type === 'show_menu' && Array.isArray(step.menu?.choices)) {
        return step.menu.choices.map((c: any) => ({
            text: c.text || '',
            targetLabelId: c.transition?.targetLabelId || '',
            transition: c.transition || { targetLabelId: c.transition?.targetLabelId || '' },
        }));
    }
    // Для старой структуры с menuRequest
    if (Array.isArray(step.menuRequest?.choices) && step.menuRequest.choices.length > 0) {
        return step.menuRequest.choices;
    }
    // Для прямого choices
    if (Array.isArray(step.choices) && step.choices.length > 0) {
        return step.choices;
    }
    return [];
};

const collectChoiceHandles = (steps: Step[]): SceneNodeData['choices'] => {
    const result: SceneNodeData['choices'] = [];

    for (const step of steps) {
        // Проверяем все варианты типов menu
        if (step.type !== 'choice' && step.type !== 'menu' && step.type !== 'show_menu') {
            continue;
        }

        const choices = extractStepChoices(step);
        choices.forEach((choice, choiceIndex) => {
            const hasText = choice.text && choice.text.trim().length > 0;
            const targetLabelId = extractChoiceTarget(choice);
            const hasTarget = targetLabelId && targetLabelId.trim().length > 0;

            if (!hasText && !hasTarget) {
                return;
            }

            result.push({
                handleId: buildChoiceHandleId(step.id, choiceIndex),
                text: choice.text || choice.name || `Вариант ${choiceIndex + 1}`,
                stepId: step.id,
                choiceIndex: choiceIndex,
            });
        });
    }

    return result;
};

const getLayoutStorageKey = (novelId: string): string => `novivovi-scenes-layout-${novelId}`;

const readStoredLayout = (novelId: string): Record<string, { x: number; y: number }> => {
    try {
        const raw = sessionStorage.getItem(getLayoutStorageKey(novelId));
        if (!raw) {
            return {};
        }
        return JSON.parse(raw) as Record<string, { x: number; y: number }>;
    } catch {
        return {};
    }
};

const saveLayout = (novelId: string, nodes: SceneNode[]): void => {
    if (nodes.length === 0) {
        return;
    }

    const layout = nodes.reduce<Record<string, { x: number; y: number }>>((acc, node) => {
        acc[node.id] = { x: node.position.x, y: node.position.y };
        return acc;
    }, {});

    try {
        sessionStorage.setItem(getLayoutStorageKey(novelId), JSON.stringify(layout));
    } catch {
        // Ignore storage write errors for session layout.
    }
};

const buildNodes = (
    labels: Label[],
    stepsByLabelId: Record<string, Step[]>,
    startLabelId?: string
): SceneNode[] => {
    const columns = Math.max(1, Math.ceil(Math.sqrt(labels.length)));

    return labels.map((label, index) => {
        const steps = stepsByLabelId[label.id] ?? [];
        return {
            id: label.id,
            type: 'scene',
            position: {
                x: (index % columns) * 320,
                y: Math.floor(index / columns) * 180,
            },
            // В buildNodes:
            data: {
                label: label.name,
                preview: findPreviewData(steps), // Сохраняем объект целиком
                choices: collectChoiceHandles(steps),
                isStart: label.id === startLabelId,
            },
        };
    });
};

const buildEdges = (
    labels: Label[],
    stepsByLabelId: Record<string, Step[]>
): SceneEdge[] => {
    const labelIds = new Set(labels.map((label) => label.id));
    const edges: SceneEdge[] = [];

    console.log('buildEdges called with:', { labels: labels.length, stepsByLabelId });

    for (const label of labels) {
        const steps = stepsByLabelId[label.id] ?? [];

        for (const step of steps) {
            // Jump step - проверяем все варианты
            if (step.type === 'jump') {
                const targetId = (step.targetId || step.targetLabelId || step.transition?.targetLabelId)?.trim();
                console.log('Processing jump step:', { stepId: step.id, targetId, step });
                if (!targetId || !labelIds.has(targetId)) {
                    console.log('Skipping jump - no target or target not found');
                    continue;
                }

                edges.push({
                    id: `jump-${label.id}-${targetId}-${step.id}`,
                    source: label.id,
                    target: targetId,
                    type: 'smoothstep',
                    sourceHandle: JUMP_SOURCE_HANDLE_ID,
                    label: 'jump',
                    style: {
                        stroke: '#111',
                        strokeWidth: 2,
                    },
                    markerEnd: {
                        type: MarkerType.ArrowClosed,
                        color: '#111',
                    },
                    data: {
                        kind: 'jump',
                        stepId: step.id,
                        sourceLabelId: label.id,
                    },
                });
                console.log('Added jump edge');
                continue;
            }

            // Menu/Choice step
            if (step.type !== 'choice' && step.type !== 'menu' && step.type !== 'show_menu') {
                continue;
            }

            const choices = extractStepChoices(step);
            console.log('Processing menu step:', { stepId: step.id, type: step.type, choices });
            choices.forEach((choice, index) => {
                const targetId = extractChoiceTarget(choice);
                console.log('Processing choice:', { index, targetId, choice });
                if (!targetId || !labelIds.has(targetId)) {
                    console.log('Skipping choice - no target or target not found');
                    return;
                }

                edges.push({
                    id: `choice-${label.id}-${targetId}-${step.id}-${choice.id ?? index}`,
                    source: label.id,
                    target: targetId,
                    type: 'smoothstep',
                    sourceHandle: buildChoiceHandleId(step.id, index),
                    label: choice.text || choice.name || `choice ${index + 1}`,
                    style: {
                        stroke: '#6b21a8',
                        strokeWidth: 2,
                    },
                    markerEnd: {
                        type: MarkerType.ArrowClosed,
                        color: '#6b21a8',
                    },
                    labelStyle: {
                        fill: '#6b21a8',
                        fontWeight: 600,
                    },
                    labelBgStyle: {
                        fill: '#f5f3ff',
                    },
                    labelBgPadding: [6, 3],
                    labelBgBorderRadius: 4,
                    data: {
                        kind: 'choice',
                        stepId: step.id,
                        choiceIndex: index,
                        sourceLabelId: label.id,
                    },
                });
                console.log('Added choice edge');
            });
        }
    }

    console.log('buildEdges result:', edges.length, 'edges');
    return edges;
};

export default function InfiniteCanvas({ novelId }: InfiniteCanvasProps) {
    const [labels, setLabels] = useState<Label[]>([]);
    const [startLabelId, setStartLabelId] = useState<string | undefined>();
    const [nodes, setNodes, onNodesChange] = useNodesState<SceneNode>([]);
    const [edges, setEdges, onEdgesChange] = useEdgesState<SceneEdge>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [stepsByLabelId, setStepsByLabelId] = useState<Record<string, Step[]>>({});

    // Маппинг stepId -> labelId для API вызовов
    const stepToLabelMap = useMemo(() => {
        const map: Record<string, string> = {};
        Object.entries(stepsByLabelId).forEach(([labelId, steps]) => {
            steps.forEach(step => {
                map[step.id] = labelId;
            });
        });
        return map;
    }, [stepsByLabelId]);

    const cloneStepsMap = useCallback((map: Record<string, Step[]>): Record<string, Step[]> => {
        return Object.entries(map).reduce<Record<string, Step[]>>((acc, [labelId, steps]) => {
            acc[labelId] = [...steps];
            return acc;
        }, {});
    }, []);

    const applyStepsMap = useCallback((nextMap: Record<string, Step[]>) => {
        setStepsByLabelId(nextMap);
        setNodes((prevNodes) =>
            prevNodes.map((node) => ({
                ...node,
                data: {
                    ...node.data,
                    preview: findPreviewData(nextMap[node.id] ?? []),
                    choices: collectChoiceHandles(nextMap[node.id] ?? []),
                },
            }))
        );
        setEdges(buildEdges(labels, nextMap));
    }, [labels, setEdges, setNodes]);

    const upsertStep = useCallback((map: Record<string, Step[]>, labelId: string, step: Step): Record<string, Step[]> => {
        const nextMap = cloneStepsMap(map);
        const current = nextMap[labelId] ?? [];
        
        // Нормализуем step перед добавлением
        const normalizedStep = { ...step };
        
        // Для jump step - извлекаем targetLabelId из transition если нужно
        if (step.type === 'jump' && !step.targetLabelId && step.transition?.targetLabelId) {
            normalizedStep.targetLabelId = step.transition.targetLabelId;
            normalizedStep.targetId = step.transition.targetLabelId;
        }
        
        // Для show_menu - извлекаем choices из menu если нужно
        if (step.type === 'show_menu' && !step.choices && step.menu?.choices) {
            normalizedStep.choices = step.menu.choices;
        }
        
        const index = current.findIndex((item) => item.id === normalizedStep.id);
        if (index === -1) {
            nextMap[labelId] = [...current, normalizedStep];
        } else {
            const nextSteps = [...current];
            nextSteps[index] = normalizedStep;
            nextMap[labelId] = nextSteps;
        }
        console.log('upsertStep result:', { labelId, step: normalizedStep, nextMap });
        return nextMap;
    }, [cloneStepsMap]);

    const removeStep = useCallback((map: Record<string, Step[]>, labelId: string, stepId: string): Record<string, Step[]> => {
        const nextMap = cloneStepsMap(map);
        nextMap[labelId] = (nextMap[labelId] ?? []).filter((step) => step.id !== stepId);
        return nextMap;
    }, [cloneStepsMap]);

    const loadGraph = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const { data: labels } = await labelsApi.getAll(novelId);

            const startLabelId = labels[0]?.id;

            const stepResponses = await Promise.all(
                labels.map((label) =>
                    stepsApi.getAll(novelId, label.id)
                )
            );

            const nextStepsByLabelId = labels.reduce<Record<string, Step[]>>((acc, label, index) => {
                acc[label.id] = stepResponses[index].data ?? [];
                return acc;
            }, {});
            const previewByLabelId = labels.reduce<Record<string, string>>((acc, label) => {
                acc[label.id] = previewImage;
                return acc;
            }, {});

            const layout = readStoredLayout(novelId);
            const nextNodes = buildNodes(labels, nextStepsByLabelId, previewByLabelId, startLabelId).map((node) => ({
                ...node,
                position: layout[node.id] ?? node.position,
            }));

            setLabels(labels);
            setStartLabelId(startLabelId);
            setStepsByLabelId(nextStepsByLabelId);
            setNodes(nextNodes);
            setEdges(buildEdges(labels, nextStepsByLabelId));
        } catch (loadError) {
            console.error(loadError);
            setLabels([]);
            setNodes([]);
            setEdges([]);
            setStepsByLabelId({});
            setError('Не удалось построить граф сцен');
        } finally {
            setLoading(false);
        }
    }, [novelId, setEdges, setNodes]);

    useEffect(() => {
        void loadGraph();
    }, [loadGraph]);

    useEffect(() => {
        saveLayout(novelId, nodes);
    }, [novelId, nodes]);

    const onConnect = useCallback(
        async (connection: Connection) => {
            const sourceLabelId = connection.source;
            const targetLabelId = connection.target;

            if (!sourceLabelId || !targetLabelId) {
                return;
            }

            console.log('onConnect called:', { sourceLabelId, targetLabelId, sourceHandle: connection.sourceHandle });

            try {
                let nextMap = cloneStepsMap(stepsByLabelId);

                if (connection.sourceHandle) {
                    const choiceInfo = parseChoiceHandleId(connection.sourceHandle);

                    if (choiceInfo) {
                        const sourceSteps = nextMap[sourceLabelId] ?? [];
                        let step = sourceSteps.find((item) => item.id === choiceInfo.stepId);
                        let choices: StepChoice[];

                        if (!step) {
                            const { data: newStep } = await stepsApi.create(novelId, sourceLabelId, {
                                type: 'menu',
                                choices: [{ 
                                    text: 'Новый выбор', 
                                    transition: { targetLabelId: targetLabelId }
                                }],
                            });
                            console.log('Created new menu step:', newStep);
                            step = newStep;
                            nextMap = upsertStep(nextMap, sourceLabelId, step);
                            choices = extractStepChoices(step);
                            const choiceIndex = 0;
                            const updatedChoices = choices.map((c, i) =>
                                i === choiceIndex ? { 
                                    ...c, 
                                    text: c.text || 'Новый выбор',
                                    transition: { targetLabelId: targetLabelId }
                                } : c
                            );
                            const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, step.id, {
                                type: 'menu',
                                choices: updatedChoices,
                            });
                            console.log('Updated menu step:', updatedStep);
                            nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                        } else {
                            choices = extractStepChoices(step);
                            if (choices[choiceInfo.choiceIndex]) {
                                const updatedChoices = choices.map((choice, index) =>
                                    index === choiceInfo.choiceIndex
                                        ? { 
                                            ...choice, 
                                            text: choice.text || `Вариант ${index + 1}`,
                                            transition: { targetLabelId: targetLabelId }
                                        }
                                        : choice
                                );
                                const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, step.id, {
                                    type: 'menu',
                                    choices: updatedChoices,
                                });
                                nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                            } else {
                                const newChoice = {
                                    text: `Вариант ${choices.length + 1}`,
                                    transition: { targetLabelId: targetLabelId },
                                };
                                const updatedChoices = [...choices, newChoice];
                                const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, step.id, {
                                    type: 'menu',
                                    choices: updatedChoices,
                                });
                                nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                            }
                        }

                        applyStepsMap(nextMap);
                        return;
                    } else {
                        const sourceSteps = nextMap[sourceLabelId] ?? [];
                        const jumpStep = sourceSteps.find((step) => step.type === 'jump');
                        if (jumpStep) {
                            const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, jumpStep.id, {
                                type: 'jump',
                                targetLabelId: targetLabelId,
                            });
                            nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                        } else {
                            const { data: createdStep } = await stepsApi.create(novelId, sourceLabelId, {
                                type: 'jump',
                                targetLabelId: targetLabelId,
                            });
                            console.log('Created jump step:', createdStep);
                            nextMap = upsertStep(nextMap, sourceLabelId, createdStep);
                        }
                    }
                } else {
                    const sourceSteps = nextMap[sourceLabelId] ?? [];
                    const jumpStep = sourceSteps.find((step) => step.type === 'jump');
                    if (jumpStep) {
                        const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, jumpStep.id, {
                            type: 'jump',
                            targetLabelId: targetLabelId,
                        });
                        console.log('Updated jump step:', updatedStep);
                        nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                    } else {
                        const { data: createdStep } = await stepsApi.create(novelId, sourceLabelId, {
                            type: 'jump',
                            targetLabelId: targetLabelId,
                        });
                        console.log('Created jump step:', createdStep);
                        nextMap = upsertStep(nextMap, sourceLabelId, createdStep);
                    }
                }

                console.log('Applying steps map:', nextMap);
                applyStepsMap(nextMap);
            } catch (connectError) {
                console.error(connectError);
                alert('Не удалось сохранить связь между сценами');
            }
        },
        [applyStepsMap, cloneStepsMap, novelId, stepsByLabelId, upsertStep]
    );

    const onEdgesDelete = useCallback(
        async (deletedEdges: Edge[]) => {
            if (deletedEdges.length === 0) {
                return;
            }

            try {
                const typedDeletedEdges = deletedEdges as SceneEdge[];
                let nextMap = cloneStepsMap(stepsByLabelId);

                for (const edge of typedDeletedEdges) {
                    const edgeData = edge.data;
                    if (!edgeData) {
                        continue;
                    }

                    if (edgeData.kind === 'jump') {
                        await stepsApi.delete(novelId, edgeData.sourceLabelId, edgeData.stepId);
                        nextMap = removeStep(nextMap, edgeData.sourceLabelId, edgeData.stepId);
                        continue;
                    }

                    if (edgeData.kind === 'choice') {
                        const sourceSteps = nextMap[edgeData.sourceLabelId] ?? [];
                        const step = sourceSteps.find((item) => item.id === edgeData.stepId);
                        if (!step) {
                            continue;
                        }

                        const choices = extractStepChoices(step);

                        if (!choices[edgeData.choiceIndex]) {
                            continue;
                        }

                        const updatedChoices = choices.filter((_, index) => index !== edgeData.choiceIndex);

                        if (updatedChoices.length === 0) {
                            await stepsApi.delete(novelId, edgeData.sourceLabelId, step.id);
                            nextMap = removeStep(nextMap, edgeData.sourceLabelId, step.id);
                        } else {
                            const { data: updatedStep } = await stepsApi.patch(novelId, edgeData.sourceLabelId, step.id, {
                                type: 'menu',
                                choices: updatedChoices.map(c => ({
                                    text: c.text || '',
                                    transition: { targetLabelId: c.targetLabelId || c.transition?.targetLabelId || '' }
                                })),
                            });
                            nextMap = upsertStep(nextMap, edgeData.sourceLabelId, updatedStep);
                        }
                    }
                }

                applyStepsMap(nextMap);
            } catch (deleteError) {
                console.error(deleteError);
                alert('Не удалось удалить связь');
            }
        },
        [applyStepsMap, cloneStepsMap, removeStep, stepsByLabelId, upsertStep]
    );

    const nodeTypes = useMemo(() => ({
        scene: SceneGraphNode,
    }), []);

    const statusView = useMemo(() => {
        if (loading) {
            return 'Загрузка графа...';
        }
        if (error) {
            return error;
        }
        if (nodes.length === 0) {
            return 'В этой новелле пока нет сцен';
        }
        return null;
    }, [error, loading, nodes.length]);

    return (
        <div className={css({
            flex: 1,
            width: '100%',
        })}>
            {statusView ? (
                <div className={css({
                    width: '100%',
                    height: '100%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '16px',
                    color: error ? '#b91c1c' : 'black',
                })}>
                    {statusView}
                </div>
            ) : (
                <ReactFlow
                    nodes={nodes}
                    edges={edges}
                    nodeTypes={nodeTypes}
                    onNodesChange={onNodesChange}
                    onEdgesChange={onEdgesChange}
                    onConnect={onConnect}
                    onEdgesDelete={onEdgesDelete}
                    fitView
                    nodesDraggable
                    nodesConnectable
                    elementsSelectable
                >
                    <Background />
                    <Controls />
                </ReactFlow>
            )}
        </div>
    );
}