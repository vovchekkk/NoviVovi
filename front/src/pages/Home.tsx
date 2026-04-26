import {css} from '../../styled-system/css'
import Header from '../shared/ui/Header'
import VideoContainer from '../shared/ui/VideoContainer'
import CTAButton from '../shared/ui/CTAButton'
import Modal from "../shared/ui/Modal.tsx";
import {useState} from "react";
import api from "../api.tsx";
import { useNavigate } from "react-router-dom";
import { setLastNovelId } from "../shared/lib/novelSession.ts";

type Novel = {
    id: string;
    title: string;
};
export default function Home() {
    const navigate = useNavigate();
    const [isOpen, setIsOpen] = useState(false);
    const [novelTitle, setNovelTitle] = useState('');
    const [createdNovelId, setCreatedNovelId] = useState<string | null>(null);
    const createNovel = async (e) => {
        e.preventDefault();
        try {
            const {data: newNovel} = await api.post<Novel>('/novels', {
                title: novelTitle
            })
            setLastNovelId(newNovel.id);
            setIsOpen(false);
            setNovelTitle('');
            setCreatedNovelId(newNovel.id);
        } catch (error) {
            console.error(error);
            alert('Не удалось создать новеллу. Попробуйте еще раз.');
        }
    }
    return (
        <div className={css({
            bg: 'background',
            minHeight: '100vh',
            color: 'white',
        })}>
            <Header active="main"/>

            <main className={css({
                pt: '90px',
                pb: '120px',
                px: '20px',
            })}>
                <VideoContainer/>

                <CTAButton onClick={() => setIsOpen(true)}/>
                <Modal active={isOpen} setActive={setIsOpen}>
                    <div className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '20px',
                    })}>

                        <form onSubmit={createNovel} className={css({
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
                                <label className={css({fontSize: '18px', textAlign: 'left'})}>Название</label>
                                <input value={novelTitle}
                                       onChange={(e) => setNovelTitle(e.target.value)}
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
                <Modal active={Boolean(createdNovelId)} setActive={() => setCreatedNovelId(null)}>
                    <div className={css({
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '16px',
                        width: '360px',
                    })}>
                        <h3 className={css({
                            fontSize: '20px',
                            fontWeight: '700',
                            margin: '0',
                        })}>
                            Новелла создана
                        </h3>
                        <p className={css({
                            margin: '0',
                            fontSize: '14px',
                            color: '#4b5563',
                        })}>
                            ID вашей новеллы:
                        </p>
                        <div className={css({
                            fontSize: '16px',
                            fontWeight: '700',
                            padding: '10px 12px',
                            borderRadius: '8px',
                            border: '1px solid #d1d5db',
                            backgroundColor: '#f9fafb',
                            wordBreak: 'break-all',
                        })}>
                            {createdNovelId}
                        </div>
                        <button
                            type="button"
                            onClick={() => {
                                if (!createdNovelId) return;
                                navigate(`/editor/${createdNovelId}`);
                                setCreatedNovelId(null);
                            }}
                            className={css({
                                padding: '10px 20px',
                                borderRadius: '8px',
                                border: 'none',
                                backgroundColor: '#705661',
                                color: 'white',
                                fontWeight: 'bold',
                                _hover: { bg: '#A87383' },
                            })}
                        >
                            Открыть редактор
                        </button>
                    </div>
                </Modal>
            </main>
        </div>
    )
}
