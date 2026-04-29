import {css} from '../../../styled-system/css'
import {type Step, stepDisplayNames, StepType} from "../../pages/Editor.tsx";

interface BlockPanelProps {
    steps: Step[];
    selectedStepIndex: number;
    onSelectStep: (index: number) => void;
    onAddClick: () => void;
    onDeleteStep: (index: number) => void;
    onMoveStep?: (fromIndex: number, toIndex: number) => void;
}

export default function BlockPanel({steps, selectedStepIndex, onSelectStep, onAddClick, onDeleteStep, onMoveStep}: BlockPanelProps) {
    return (
        <div className={css({
            width:'70%',
            minHeight: '10em',
            height: 'auto',
            margin: '0 auto',
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#DFC6D1',
            borderRadius: '12px',
        })}>
            <button
                onClick={onAddClick}
                className={css({
                    bg: 'white',
                    width:'220px',
                    minHeight: '44px',
                    padding:'10px 16px',
                    margin: '10px',
                    border: '2px solid #705661',
                    _hover: {bg: '#DFC6D1',
                        boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'},
                    borderRadius: '12px',
                    fontWeight: 'medium',
                    flexShrink: 0,
                })}
            >
                + Добавить блок
            </button>
            <div className={css({
                minHeight: '10em',
                height: 'auto',
                display: 'flex',
                flexDirection: 'column',
            })}>
                {steps.map((step, index) => {
                    const isSelected = index === selectedStepIndex;
                    return (
                        <div key={step.id}
                             onClick={() => onSelectStep(index)}
                             className={css({
                                 height: '40%',
                                 display: 'flex',
                                 alignItems: 'stretch',
                                 padding: '4px',
                                 margin: '5px',
                                 backgroundColor: '#F8EDEB',
                                 border:isSelected ? '2px solid #705661' : 'transparent',
                                 borderRadius: '12px',
                                 cursor: 'pointer',
                                 justifyContent: 'space-between',
                             })}>
                            <div className={css({ display: 'flex', flex: 1 })}>
                                <div className={css({
                                    backgroundColor: '#705661',
                                    color: 'white',
                                    width: '52px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    borderRadius: '12px',
                                    flexShrink: 0,
                                    marginRight: '5px',
                                })}>
                                    {index + 1}
                                </div>
                                <div className={css({
                                    display: 'flex',
                                    alignItems: 'center',
                                    flex: 1,
                                    paddingLeft: '8px',
                                })}>
                                    {stepDisplayNames[step.type as StepType] || step.type}
                                </div>
                            </div>
                            <div className={css({ display: 'flex', gap: '4px', alignItems: 'center' })}>
                                {onMoveStep && index > 0 && (
                                    <button
                                        onClick={(e) => {
                                            e.stopPropagation();
                                            onMoveStep(index, index - 1);
                                        }}
                                        className={css({
                                            backgroundColor: '#705661',
                                            color: 'white',
                                            width: '32px',
                                            height: '32px',
                                            borderRadius: '8px',
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: 'center',
                                            cursor: 'pointer',
                                            _hover: { backgroundColor: '#8B7178' },
                                        })}
                                        title="Переместить вверх"
                                    >
                                        ↑
                                    </button>
                                )}
                                {onMoveStep && index < steps.length - 1 && (
                                    <button
                                        onClick={(e) => {
                                            e.stopPropagation();
                                            onMoveStep(index, index + 1);
                                        }}
                                        className={css({
                                            backgroundColor: '#705661',
                                            color: 'white',
                                            width: '32px',
                                            height: '32px',
                                            borderRadius: '8px',
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: 'center',
                                            cursor: 'pointer',
                                            _hover: { backgroundColor: '#8B7178' },
                                        })}
                                        title="Переместить вниз"
                                    >
                                        ↓
                                    </button>
                                )}
                                <button
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        onDeleteStep(index);
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
                                        _hover: { backgroundColor: '#dc2626' },
                                    })}
                                    title="Удалить"
                                >
                                    ×
                                </button>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}
