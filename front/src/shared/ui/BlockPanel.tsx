import {css} from '../../../styled-system/css'
import Block from "./Block.tsx";
import {type Step, stepDisplayNames, StepType} from "../../pages/Editor.tsx";

interface BlockPanelProps {
    steps: Step[];
    selectedStepIndex: number;
    onSelectStep: (index: number) => void;
    onAddClick: () => void;
}

export default function BlockPanel({steps, selectedStepIndex, onSelectStep, onAddClick}: BlockPanelProps) {
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
                    width:'20%',
                    height:'auto',
                    padding:'10px',
                    margin: '10px',
                    border: '2px solid #705661',
                    _hover: {bg: '#DFC6D1',
                        boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)'},
                    borderRadius: '12px',
                    fontWeight: 'medium',
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
                             })}>
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
                                padding: '10px',
                                backgroundColor: 'white',
                                borderRadius: '12px',
                                flex:1,
                            })}>
                                {stepDisplayNames[step.type as StepType]}
                            </div>
                        </div>
                    );
                })}

                {steps.length === 0 && (
                    <div
                        className={css({
                            textAlign: 'center',
                            py: '40px',
                            color: '#8A6A7A',
                            fontStyle: 'italic',
                        })}
                    >
                        Пока нет шагов. Добавьте первый блок!
                    </div>
                )}
            </div>
        </div>
    )
}