import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';
import {css} from '../../../styled-system/css'

interface ModalProps {
    active: boolean;
    setActive: (active: boolean) => void;
    children: React.ReactNode;
}

const Modal = ({ active, setActive, children }: ModalProps) => {
    useEffect(() => {
        if (active) {
            document.body.style.overflow = 'hidden';
            const handleEsc = (e: KeyboardEvent) => {
                if (e.key === 'Escape') setActive(false);
            };
            window.addEventListener('keydown', handleEsc);
            return () => {
                window.removeEventListener('keydown', handleEsc);
                document.body.style.overflow = 'unset';
            };
        }
    }, [active, setActive]);

    if (!active) return null;

    return createPortal(
        <div
            className={css({
                position: 'fixed',
                top: 0,
                left: 0,
                width: '100vw',
                height: '100vh',
                backgroundColor: 'rgba(0, 0, 0, 0.5)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                zIndex: 1000,
                animation: 'fadeIn 0.2s ease-out',
            })}
            onClick={() => setActive(false)}
        >
            <div
                className={css({
                    padding: '6',
                    borderRadius: 'xl',
                    backgroundColor: 'white',
                    minWidth: '320px',
                    maxWidth: '90%',
                    position: 'relative',
                    boxShadow: '2xl',
                    border: '1px solid',
                    borderColor: 'gray.200',
                    animation: 'slideUp 0.3s ease-out',
                })}
                onClick={(e) => e.stopPropagation()}
            >
                <button
                    className={css({
                        position: 'absolute',
                        top: '2',
                        right: '4',
                        fontSize: '2xl',
                        cursor: 'pointer',
                        background: 'none',
                        border: 'none',
                        color: 'gray.500',
                        _hover: {
                            color: 'red.500',
                        }
                    })}
                    onClick={() => setActive(false)}
                >
                    &times;
                </button>
                {children}
            </div>
        </div>,
        document.getElementById('modal-root')!
    );
};
export default Modal