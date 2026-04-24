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

const nodeStyles = css({
    height: '100px',
    padding: '12px 16px',
    fontSize: '16px',
    border: '2px solid token(colors.blue.600)',
    borderRadius: '8px',
    backgroundColor: 'white',
    boxShadow: '0 4px 12px -2px token(colors.gray.200)',
    minWidth: '180px',
    _hover: {borderColor: 'token(colors.blue.500)'},
});
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

const ChoiceNode = ({data}: any) => {
    return (
        <div className={choiceNodeStyles}>
            {/* Вход слева */}
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

const CustomNode = ({data}: any) => {
    return (
        <div className={nodeStyles}>
            <Handle type="target" position={Position.Left}
                    style={{background: '#3b82f6', width: '8px', height: '8px'}}/>
            <div>{data.label}</div>
            <Handle type="source" position={Position.Right}
                    style={{background: '#3b82f6', width: '8px', height: '8px'}}/>
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


export default function InfiniteCanvas() {
    const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
    const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
    const onConnect = useCallback(
        (params) => setEdges((eds) => addEdge(params, eds)),
        [setEdges]
    );
    // useEffect(() => {
    //     const fetchLabels = async () => {
    //         try {
    //             // setLoading(true);
    //             const {data} = await api.get<Label[]>('/novels/1/labels');
    //             if (data)
    //             setNodes(data);
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
        custom: CustomNode,
        choice: ChoiceNode,
    }), []);
    const createLabel = () => {
        alert('создание новой сцены');
    }
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
                    </div>
                </Panel>
            </ReactFlow>
        </div>
    );
}
