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
    return (
        <div className={css({
            width: '220px',
            border: `2px solid ${data.isStart ? '#f59e0b' : '#775D68'}`,
            borderRadius: '12px',
            backgroundColor: 'white',
            overflow: 'hidden',
            boxShadow: 'md',
            position: 'relative',
        })}>
            {data.isStart && (
                <div className={css({
                    position: 'absolute',
                    top: '-10px',
                    left: '-10px',
                    backgroundColor: '#f59e0b',
                    borderRadius: '50%',
                    width: '24px',
                    height: '24px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '14px',
                    zIndex: 10,
                    boxShadow: '0 2px 4px rgba(0,0,0,0.2)',
                })}>
                    🚀
                </div>
            )}
            <Handle
                type="target"
                position={Position.Left}
                style={{ background: '#111', width: 10, height: 10 }}
            />
            <Handle
                type="source"
                position={Position.Right}
                id={JUMP_SOURCE_HANDLE_ID}
                title="jump"
                style={{ background: '#111', width: 10, height: 10, top: '24px' }}
            />
            <img
                src={data.previewUrl}
                alt={data.label}
                className={css({
                    width: '100%',
                    height: '120px',
                    objectFit: 'cover',
                    borderBottom: '1px solid #e5e7eb',
                })}
            />
            <div className={css({
                padding: '10px 12px',
                fontWeight: '700',
                textAlign: 'center',
            })}>
                {data.label}
            </div>
            {data.choices.length > 0 && (
                <div className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '6px',
                    padding: '0 10px 10px 10px',
                })}>
                    {data.choices.map((choice) => (
                        <div
                            key={choice.handleId}
                            className={css({
                                position: 'relative',
                                padding: '6px 12px',
                                border: '1px solid #d1d5db',
                                borderRadius: '8px',
                                fontSize: '12px',
                                backgroundColor: '#faf5ff',
                            })}
                        >
                            {choice.text}
                            <Handle
                                type="source"
                                position={Position.Right}
                                id={choice.handleId}
                                style={{
                                    background: '#6b21a8',
                                    width: 10,
                                    height: 10,
                                    right: -6,
                                    top: '50%',
                                    transform: 'translateY(-50%)',
                                }}
                            />
                        </div>
                    ))}
                </div>
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
            const hasTarget = choice.targetLabelId && choice.targetLabelId.trim().length > 0;

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
    previewByLabelId: Record<string, string>,
    startLabelId?: string
): SceneNode[] => {
    const columns = Math.max(1, Math.ceil(Math.sqrt(labels.length)));

    return labels.map((label, index) => ({
        id: label.id,
        type: 'scene',
        position: {
            x: (index % columns) * 320,
            y: Math.floor(index / columns) * 180,
        },
        data: {
            label: label.name,
            previewUrl: previewByLabelId[label.id] ?? previewImage,
            choices: collectChoiceHandles(stepsByLabelId[label.id] ?? []),
            isStart: label.id === startLabelId,
        },
    }));
};

const buildEdges = (
    labels: Label[],
    stepsByLabelId: Record<string, Step[]>
): SceneEdge[] => {
    const labelIds = new Set(labels.map((label) => label.id));
    const edges: SceneEdge[] = [];

    for (const label of labels) {
        const steps = stepsByLabelId[label.id] ?? [];

        for (const step of steps) {
            // Jump step - проверяем оба варианта (старый targetId и новый targetLabelId)
            if (step.type === 'jump') {
                const targetId = (step.targetId || step.targetLabelId)?.trim();
                if (!targetId || !labelIds.has(targetId)) {
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
                continue;
            }

            // Menu/Choice step
            if (step.type !== 'choice' && step.type !== 'menu' && step.type !== 'show_menu') {
                continue;
            }

            const choices = extractStepChoices(step);
            choices.forEach((choice, index) => {
                const targetId = extractChoiceTarget(choice);
                if (!targetId || !labelIds.has(targetId)) {
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
            });
        }
    }

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
                    choices: collectChoiceHandles(nextMap[node.id] ?? []),
                },
            }))
        );
        setEdges(buildEdges(labels, nextMap));
    }, [labels, setEdges, setNodes]);

    const upsertStep = useCallback((map: Record<string, Step[]>, labelId: string, step: Step): Record<string, Step[]> => {
        const nextMap = cloneStepsMap(map);
        const current = nextMap[labelId] ?? [];
        const index = current.findIndex((item) => item.id === step.id);
        if (index === -1) {
            nextMap[labelId] = [...current, step];
        } else {
            const nextSteps = [...current];
            nextSteps[index] = step;
            nextMap[labelId] = nextSteps;
        }
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
                                type: 'show_menu',
                                choices: [{ text: 'Новый выбор', targetLabelId: targetLabelId }],
                            });
                            step = newStep;
                            nextMap = upsertStep(nextMap, sourceLabelId, step);
                            choices = extractStepChoices(step);
                            const choiceIndex = 0;
                            const updatedChoices = choices.map((c, i) =>
                                i === choiceIndex ? { ...c, targetLabelId: targetLabelId } : c
                            );
                            const { data: updatedStep } = await stepsApi.patch(novelId, sourceLabelId, step.id, {
                                type: 'show_menu',
                                choices: updatedChoices,
                            });
                            nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                        } else {
                            choices = extractStepChoices(step);
                            if (choices[choiceInfo.choiceIndex]) {
                                const updatedChoices = choices.map((choice, index) =>
                                    index === choiceInfo.choiceIndex
                                        ? { ...choice, targetLabelId: targetLabelId }
                                        : choice
                                );
                                const { data: updatedStep } = await api.patch<Step>(`/steps/${step.id}`, {
                                    ...step,
                                    menuRequest: { id: step.menuRequest?.id ?? `menu-${step.id}`, choices: updatedChoices },
                                });
                                nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                            } else {
                                const newChoice: StepChoice = {
                                    text: `Вариант ${choices.length + 1}`,
                                    targetLabelId: targetLabelId,
                                };
                                const updatedChoices = [...choices, newChoice];
                                const { data: updatedStep } = await api.patch<Step>(`/steps/${step.id}`, {
                                    ...step,
                                    menuRequest: { id: step.menuRequest?.id ?? `menu-${step.id}`, choices: updatedChoices },
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
                            const { data: updatedStep } = await api.patch<Step>(`/steps/${jumpStep.id}`, {
                                ...jumpStep,
                                targetId: targetLabelId,
                            });
                            nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                        } else {
                            const { data: createdStep } = await api.post<Step>(`/novels/${novelId}/labels/${sourceLabelId}/steps`, {
                                type: 'jump',
                                targetId: targetLabelId,
                            });
                            nextMap = upsertStep(nextMap, sourceLabelId, createdStep);
                        }
                    }
                } else {
                    const sourceSteps = nextMap[sourceLabelId] ?? [];
                    const jumpStep = sourceSteps.find((step) => step.type === 'jump');
                    if (jumpStep) {
                        const { data: updatedStep } = await api.patch<Step>(`/steps/${jumpStep.id}`, {
                            ...jumpStep,
                            targetId: targetLabelId,
                        });
                        nextMap = upsertStep(nextMap, sourceLabelId, updatedStep);
                    } else {
                        const { data: createdStep } = await api.post<Step>(`/novels/${novelId}/labels/${sourceLabelId}/steps`, {
                            type: 'jump',
                            targetId: targetLabelId,
                        });
                        nextMap = upsertStep(nextMap, sourceLabelId, createdStep);
                    }
                }

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
                        await api.delete(`/steps/${edgeData.stepId}`);
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
                            await api.delete(`/steps/${step.id}`);
                            nextMap = removeStep(nextMap, edgeData.sourceLabelId, step.id);
                        } else {
                            const { data: updatedStep } = await api.patch<Step>(`/steps/${step.id}`, {
                                ...step,
                                menuRequest: {
                                    id: step.menuRequest?.id ?? `menu-${step.id}`,
                                    choices: updatedChoices,
                                },
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
                    // Временно отключено: требует переделки под новое API
                    // onConnect={onConnect}
                    // onEdgesDelete={onEdgesDelete}
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