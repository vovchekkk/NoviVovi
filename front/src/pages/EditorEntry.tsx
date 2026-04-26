import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { css } from '../../styled-system/css';
import Modal from '../shared/ui/Modal';
import Header from '../shared/ui/Header';
import { getLastNovelId, setLastNovelId } from '../shared/lib/novelSession.ts';

export default function EditorEntry() {
    const navigate = useNavigate();
    const [novelId, setNovelId] = useState(getLastNovelId() ?? '');
    const [isOpen, setIsOpen] = useState(true);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const trimmedNovelId = novelId.trim();
        if (trimmedNovelId) {
            setLastNovelId(trimmedNovelId);
            navigate(`/editor/${trimmedNovelId}`);
            setIsOpen(false);
        }
    };

    const handleClose = () => {
        setIsOpen(false);
        navigate('/');
    };

    return (
        <div className={css({
            bg: 'background',
            minHeight: '100vh',
            color: 'white',
        })}>
            <Header active="editor" />

            <Modal active={isOpen} setActive={handleClose}>
                <div className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '20px',
                    width: '350px',
                })}>
                    <h2 className={css({
                        fontSize: '18px',
                        fontWeight: 'bold',
                        margin: '0',
                    })}>
                        Введите код вашей новеллы
                    </h2>

                    <form onSubmit={handleSubmit} className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '16px',
                    })}>
                        <input
                            type="text"
                            value={novelId}
                            onChange={(e) => setNovelId(e.target.value)}
                            placeholder="Например: 65a3f1b2c9d8e0f2g3h4i5j6"
                            required
                            autoFocus
                            className={css({
                                width: '100%',
                                padding: '12px',
                                borderRadius: '8px',
                                backgroundColor: 'white',
                                color: 'black',
                                border: '1px solid #ccc',
                                fontSize: '14px',
                                outline: 'none',
                                ':focus': {
                                    borderColor: '#705661',
                                    boxShadow: '0 0 0 2px rgba(112, 86, 97, 0.1)',
                                }
                            })}
                        />

                        <button
                            type="submit"
                            className={css({
                                padding: '12px 20px',
                                borderRadius: '8px',
                                border: 'none',
                                backgroundColor: '#705661',
                                color: 'white',
                                fontWeight: 'bold',
                                cursor: 'pointer',
                                fontSize: '14px',
                                transition: 'background-color 0.2s',
                                _hover: {
                                    bg: '#A87383',
                                }
                            })}
                        >
                            Открыть редактор
                        </button>

                        <button
                            type="button"
                            onClick={handleClose}
                            className={css({
                                padding: '10px 20px',
                                borderRadius: '8px',
                                border: '1px solid #ccc',
                                backgroundColor: 'transparent',
                                color: 'black',
                                fontWeight: '500',
                                cursor: 'pointer',
                                fontSize: '14px',
                                _hover: {
                                    bg: '#f0f0f0',
                                }
                            })}
                        >
                            Отмена
                        </button>
                    </form>
                </div>
            </Modal>
        </div>
    );
}
