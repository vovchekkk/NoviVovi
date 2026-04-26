import {
    ReactFlow,
    Background,
    Controls,
    useNodesState,
    useEdgesState,
    addEdge,
    Position,
    Handle,
    Panel
} from '@xyflow/react';
import {css} from '../../../styled-system/css'
import '@xyflow/react/dist/style.css';
import {useCallback, useEffect, useMemo} from "react";
import startImage from '../../assets/img_1.png';
import previewImage from '../../assets/img.png';
import api from "../../api.tsx";
import type {Label, Step} from "../../pages/Editor.tsx";
import {data} from "react-router-dom";


const choiceNodeStyles = css({
    bg: 'white',
    border: '3px solid black',
    borderRadius: '24px',
    padding: '12px',
    width: '320px',
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
});

const imageStyles = css({
    width: '100%',
    borderRadius: '16px',
    border: '2px solid #333',
    objectFit: 'cover',
});

const optionStyles = css({
    border: '2px solid black',
    borderRadius: '10px',
    padding: '8px 12px',
    fontSize: '14px',
    fontWeight: 'bold',
    position: 'relative',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
});

const ChoiceNode = ({data, selected}: any) => {
    return (
        <div className={css({
            bg: 'white',
            border: selected ? '3px solid #DFC6D1':'3px solid black',
            borderRadius: '24px',
            padding: '12px',
            width: '320px',
            display: 'flex',
            flexDirection: 'column',
            gap: '12px',
            shadow: selected ? '0 0 20px rgba(219, 132, 170, 0.5)' : 'none',
        })}>
            {/* Вход слева */}
            <Handle
                type="target"
                position={Position.Left}
                style={{background: 'black', width: '12px', height: '12px', left: '-6px'}}
            />
            {/*<Handle*/}
            {/*    type="source"*/}
            {/*    position={Position.Right}*/}
            {/*    style={{background: 'black', width: '12px', height: '12px', right: '-6px'}}*/}
            {/*/>*/}
            <div className={css({
                fontWeight: 'bold',
                textAlign: 'center',
            })}>{data.labelName}</div>
            <img src={data.previewUrl} className={imageStyles} alt="preview"/>
            <div className={css({display: 'flex', flexDirection: 'column', gap: '8px'})}>
                {data.choices.map((choice: any, index: number) => (
                    <div key={index} className={optionStyles}>
                        {choice.text}

                        <Handle
                            type="source"
                            position={Position.Right}
                            id={`choice-${index}`}
                            style={{
                                background: 'black',
                                width: '14px',
                                height: '14px',
                            }}
                        />
                    </div>
                ))}
            </div>
        </div>
    );
};

const RegularNode = ({data, selected}: any) => {
    return (
        <div className={css({
            bg: 'white',
            border: selected ? '3px solid #DFC6D1':'3px solid black',
            borderRadius: '24px',
            padding: '12px',
            width: '320px',
            display: 'flex',
            flexDirection: 'column',
            gap: '12px',
            shadow: selected ? '0 0 20px rgba(219, 132, 170, 0.5)' : 'none',
        })}>
            <Handle
                type="target"
                position={Position.Left}
                style={{background: 'black', width: '12px', height: '12px', left: '-6px'}}
            />
            <Handle
                type="source"
                position={Position.Right}
                style={{background: 'black', width: '12px', height: '12px', right: '-6px'}}
            />
            <div className={css({
                fontWeight: 'bold',
                textAlign: 'center',
            })}>{data.labelName}</div>
            <img src={data.previewUrl} className={imageStyles} alt="preview"/>
        </div>
    );
};

const StartNode = ({data}: any) => {
    return (
        <div className={css({
            width: '80px',
            height: '80px',
            borderRadius: 'full',
            border: '3px solid',
            borderColor: 'black',
            backgroundColor: 'white',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            overflow: 'hidden',
            boxShadow: 'xl',
            position: 'relative',
            _hover: {scale: 1.05, transition: 'transform 0.2s'},
        })}>
            <img
                src={data.imageUrl}
                alt="start"
                className={css({width: '100%', height: '100%', objectFit: 'cover'})}
            />
            <Handle
                type="source"
                position={Position.Right}
                className={css({background: 'black', width: '10px!', height: '10px!',})}
            />
        </div>
    );
};

type StartNodeData = {
    imageUrl: string;
    labelName: string;
};

type ChoiceNodeData = {
    previewUrl: string;
    choices: { text: string }[];
};

type AppNodeData = StartNodeData | ChoiceNodeData;

type AppNode = Node<AppNodeData>;
const initialNodes = [
    {
        id: 'start-1',
        type: 'start',
        position: {x: 0, y: 0},
        data: {imageUrl: startImage},
    },
    {
        id: '2',
        type: 'choice',
        position: {x: 250, y: 50},
        data: {
            labelName: 'Сцена 1',
            previewUrl: previewImage,
            choices: [
                {text: 'Пойти в парк'},
                {text: 'Уйти'}
            ]
        },
    },
];


const initialEdges = [
    {id: 'e1-2', source: 'start-1', target: '2', style: {stroke: 'black', strokeWidth: 3}},
];

type Node = {
    id: string;
    name: string;
}

type Edge = {
    type: string,
    id: string;
    sourceLabelId: string;
    targetLabelId: string;
    choiceId?: string;
    text?: string;
}

type GraphData = {
    nodes: Node[];
    edges: Edge[];
}

export default function InfiniteCanvas() {
    const [nodes, setNodes, onNodesChange] = useNodesState<AppNode>(initialNodes);
    const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
    const onConnect = useCallback(
        (params) => setEdges((eds) => addEdge(params, eds)),
        [setEdges]
    );
    // useEffect(() => {
    //     const fetchLabels = async () => {
    //         try {
    //             // setLoading(true);
    //             const {data} = await api.get<GraphData>('/novels/1/graph');
    //             if (data.nodes) {
    //                 for(const node of data.nodes) {
    //
    //                 }
    //             }
    //         } catch (error) {
    //             console.error(error);
    //             alert('Не удалось загрузить шаги');
    //         } finally {
    //             // setLoading(false);
    //         }
    //     };
    //
    //     fetchLabels();
    // }, []);
    const nodeTypes = useMemo(() => ({
        start: StartNode,
        regular: RegularNode,
        choice: ChoiceNode,
    }), []);
    const createLabel = async () => {
        try {
            // const {data: newLabel} = await api.post<Label>('/novels/1/labels', {
            //     name: 'Новая нода',
            // })
            const node = {
                type: 'regular',
                id: String(Date.now()),
                position: {x: 100, y: 100},
                data: {
                    labelName: 'newLabel.name',
                    previewUrl: previewImage,
                    choices: [
                        {text: 'Пойти в парк'},
                        {text: 'Уйти'}
                    ]
                },
            }
            setNodes(prev => [...prev, node]);
        } catch (error) {
            console.error(error);
            alert('Не удалось создать сцену');
        }
    }
    const onNodesDelete = useCallback(
        (deleted) => {
            console.log('Удалены ноды:', deleted);
        },
        []
    );
    return (
        <div className={css({
            flex: 1,
            width: '100%',
        })}>
            <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                onNodesChange={onNodesChange}
                onEdgesChange={onEdgesChange}
                onConnect={onConnect}
                fitView
                nodesDraggable
                nodesConnectable
                elementsSelectable
                onNodesDelete={onNodesDelete}
                deleteKeyCode={['Backspace', 'Delete']}
            >
                <Background/>
                <Controls/>
                <Panel position="top-left">
                    <div className={css({display: 'flex', gap: '10px'})}>
                        <button
                            onClick={createLabel}
                            className={css({
                                border: '2px solid #775D68',
                                padding: '10px',
                                backgroundColor: 'white',
                                borderRadius: '8px',
                                _hover: {
                                    bg: '#DFC6D1',
                                    boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                },
                            })}>
                            + Добавить сцену
                        </button>
                        <button
                            onClick={onNodesDelete}
                            className={css({
                                border: '2px solid #775D68',
                                padding: '10px',
                                backgroundColor: 'white',
                                borderRadius: '8px',
                                _hover: {
                                    bg: '#DFC6D1',
                                    boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'
                                },
                            })}>
                            Удалить сцену
                        </button>
                    </div>
                </Panel>
            </ReactFlow>
        </div>
    );
}
