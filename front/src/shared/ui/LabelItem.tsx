import type {Label} from "../../pages/Editor.tsx";
import {css} from '../../../styled-system/css'
import {useEffect, useState} from "react";
import {vstack, hstack} from '../../../styled-system/patterns';
import {MoreVertical} from 'lucide-react';
import Modal from "./Modal.tsx";

interface LabelItemProps {
    changeLabel: (id: string) => void,
    label: Label,
    selectedId: string | null,
    onDelete: (id: string) => void,
    onPatch: (label: Label) => void,
    labelName: string,
    setLabelName: (name: string) => void,
}

export function LabelItem({
                              changeLabel,
                              label,
                              selectedId,
                              onDelete,
                              onPatch,
                              setLabelName
                          }: LabelItemProps) {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [isOpen, setIsOpen] = useState(false);
    const [localName, setLocalName] = useState(label.name);
    const handleSubmit = (e) => {
        e.preventDefault();
        e.stopPropagation();
        setLabelName(localName);
        const updatedLabel = { ...label, name: localName };
        onPatch(updatedLabel);
        setIsOpen(false);
        console.log('Отправилось')
    }
    return (
        <div
            key={label.id}
            onClick={() => changeLabel(label.id)}
            className={css({
                position: 'relative',
                padding: '3px',
                width: '80%',
                borderRadius: '8px',
                backgroundColor: selectedId === label.id ? '#775D68' : 'white',
                zIndex: isMenuOpen ? 100 : 1,
                _hover: {
                    bg: '#775D68',
                    color: 'background',
                    borderColor: '#775D68',
                    transform: 'translateY(-2px)',
                    boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)',
                },
            })}
        >
            <div className={hstack({gap: '1px', alignItems: 'center', flex: 1})}>
                <p className={css({
                    fontWeight: 'bold',
                    minW: '0',
                    fontSize: '20px',
                    padding: '10px',
                    backgroundColor: 'white',
                    borderRadius: '12px',
                    w: 'full',
                })}>
                    {label.name}
                </p>
                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        setIsMenuOpen(!isMenuOpen);
                    }}
                    className={css({
                        position: 'absolute',
                        right: '8px',
                        padding: '4px',
                        borderRadius: '6px',
                        color: '#775D68',
                        _hover: {bg: 'gray.100'},
                        cursor: 'pointer'
                    })}
                >
                    <MoreVertical size={20}/>
                </button>

                {isMenuOpen && (
                    <>
                        <div
                            onClick={() => setIsMenuOpen(false)}
                            className={css({position: 'fixed', inset: 0, zIndex: 10})}
                        />

                        <div className={css({
                            position: 'absolute',
                            top: '50%',
                            right: '0',
                            mt: '2',
                            width: '160px',
                            backgroundColor: 'white',
                            border: '1px solid',
                            borderColor: 'gray.200',
                            borderRadius: 'lg',
                            boxShadow: 'xl',
                            zIndex: 50,
                            overflow: 'hidden',
                            display: 'flex',
                            flexDirection: 'column',
                        })}>
                            <button
                                className={css({
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '8px',
                                    width: '100%',
                                    padding: '12px 14px',
                                    fontSize: 'sm',
                                    textAlign: 'left',
                                    cursor: 'pointer',
                                    transition: 'background 0.2s',
                                    color: 'gray.700',
                                    _hover: {bg: 'gray.50'}
                                })}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    setLocalName(label.name);
                                    setIsOpen(true);
                                    setIsMenuOpen(false);
                                }}
                            >
                                Переименовать
                            </button>

                            <button
                                className={css({
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '8px',
                                    width: '100%',
                                    padding: '12px 14px',
                                    fontSize: 'sm',
                                    textAlign: 'left',
                                    cursor: 'pointer',
                                    transition: 'background 0.2s',
                                    color: 'red.500',
                                    _hover: {bg: 'red.50'}
                                })}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    if (window.confirm('Удалить сцену?')) {
                                        onDelete(label.id);
                                    }
                                    setIsMenuOpen(false);
                                }}
                            >
                                Удалить
                            </button>
                        </div>
                    </>
                )}
            </div>
            <Modal active={isOpen} setActive={setIsOpen}>
                <div
                    onClick={(e) => e.stopPropagation()}
                    className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '20px',
                })}>

                    <form onSubmit={handleSubmit} className={css({
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
                            <input value={localName}
                                   onChange={(e) => setLocalName(e.target.value)}
                                   required
                                   onClick={(e) => e.stopPropagation()}
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
                            Переименовать
                        </button>
                    </form>
                </div>
            </Modal>
        </div>

    )
}