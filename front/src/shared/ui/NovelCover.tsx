import { useEffect, useState } from "react";
import api from "../../api.tsx";
import { css } from "../../../styled-system/css";

interface Props {
    novelId: string;
    labelId: string;
    title?: string;
}

export function NovelCover({ novelId, labelId, title }: Props) {
    const [imgUrl, setUrl] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const getFirstBackground = async () => {
            if (!labelId) {
                setLoading(false);
                return;
            }
            try {
                // Запрашиваем все шаги стартовой сцены
                const { data } = await api.get(`/novels/${novelId}/labels/${labelId}/steps`);

                // Ищем первый шаг, где тип "show_background"
                const bgStep = data.find((step: any) => step.type === 'show_background');

                if (bgStep?.backgroundObject?.image?.url) {
                    setUrl(bgStep.backgroundObject.image.url);
                }
            } catch (e) {
                console.error("Не удалось загрузить обложку", e);
            } finally {
                setLoading(false);
            }
        };

        getFirstBackground();
    }, [novelId, labelId]);

    if (loading) {
        return <div className={css({ width: '100%', height: '100%', bg: '#333', borderRadius: '20px' })} />;
    }

    const containerStyle = css({
        width: '100%',
        height: '100%',
        borderRadius: '20px',
        overflow: 'hidden',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        boxShadow: '0 4px 20px rgba(0,0,0,0.4)',
        border: '1px solid rgba(255,255,255,0.1)'
    });
    if (imgUrl) {
        return (
            <div className={containerStyle}>
                <img
                    src={imgUrl}
                    className={css({ width: '100%', height: '100%', objectFit: 'cover' })}
                    alt="Cover"
                />
            </div>
        );
    }

    // Если фона нет — возвращаем черный фон с текстом
    return (
        <div className={containerStyle} style={{
            backgroundColor: '#000',
            flexDirection: 'column',
            padding: '20px',
            textAlign: 'center'
        }}>
            <span className={css({
                fontSize: '14px',
                color: '#705661',
                fontWeight: 'bold',
                textTransform: 'uppercase',
                letterSpacing: '2px',
                marginBottom: '10px'
            })}>
                Visual Novel
            </span>
            <span className={css({
                fontSize: '18px',
                color: 'white',
                opacity: 0.8,
                fontStyle: 'italic'
            })}>
                {title}
            </span>
        </div>
    );
}