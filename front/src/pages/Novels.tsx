import { useEffect, useState } from "react";
import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css';
import NovelCard from "../shared/ui/NovelCard.tsx";
import api from "../api.tsx";
import {novelsApi} from "../shared/api/client.ts";
import {NovelCover} from "../shared/ui/NovelCover.tsx"; // Путь к твоему настроенному axios

// Описываем тип данных, которые приходят с бэка
interface Novel {
    id: string;
    title: string;
    startLabelId: string;
    labelIds: string[];
    characterIds: string[];
}

export default function Novels() {
    const [novels, setNovels] = useState<Novel[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchNovels = async () => {
            try {
                const { data } = await novelsApi.getAll();
                setNovels(data);
            } catch (error) {
                console.error("Ошибка при загрузке новелл:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchNovels();
    }, []);

    return (
        <div className={css({
            bg: 'background', // Темный цвет фона как на скриншоте
            minHeight: '100vh',
            color: 'white',
        })}>
            <Header active="novels" />

            <main className={css({
                pt: '120px', // Отступ сверху под хедер
                pb: '80px',
                px: { base: '20px', md: '10%' }, // Динамические отступы по бокам
            })}>

                <div className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '40px', // Расстояние между карточками
                    maxWidth: '1200px',
                    margin: '0 auto',
                })}>

                    {loading ? (
                        <p className={css({ textAlign: 'center', opacity: 0.5 })}>Загрузка...</p>
                    ) : (
                        novels.map((novel) => (
                            <NovelCard
                                key={novel.id}
                                id={novel.id}
                                cover={<NovelCover
                                    novelId={novel.id}
                                    labelId={novel.startLabelId}
                                    title={novel.title || "Без названия"} // Передаем название сюда
                                />}
                                title={novel.title || "Без названия"}
                                description={`Сцен: ${novel.labelIds?.length || 0} | Персонажей: ${novel.characterIds?.length || 0}`}
                            />
                        ))
                    )}

                    {!loading && novels.length === 0 && (
                        <p className={css({ textAlign: 'center', opacity: 0.5 })}>У вас пока нет созданных новелл.</p>
                    )}
                </div>
            </main>
        </div>
    );
}