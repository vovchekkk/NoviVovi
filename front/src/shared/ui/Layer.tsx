import { useEffect, useState } from "react";
import { css } from "../../../styled-system/css";
import api from "../../api";

interface LayerProps {
    id: string | undefined | null; // Разрешаем undefined для проверок
    transform?: {
        x?: number;
        y?: number;
        width?: number;
        height?: number;
        scale?: number;
        rotation?: number;
        zIndex?: number;
    };
    className?: string;
}

export const Layer = ({ id, transform, className }: LayerProps) => {
    const [url, setUrl] = useState<string | null>(null);

    useEffect(() => {
        // Если ID пропал (селектор очищен) — СБРАСЫВАЕМ URL
        if (!id) {
            setUrl(null);
            return;
        }

        api.get<{ url: string }>(`/images/${id}`)
            .then((res) => {
                setUrl(res.data.url);
            })
            .catch(() => {
                setUrl(null);
            });

    }, [id]);

    // Если нет URL или ID — НИЧЕГО не рисуем (компонент вернет null)
    if (!id || !url) return null;

    const style: React.CSSProperties = {
        position: 'absolute',
        left: `${transform?.x ?? 0}%`,
        top: `${transform?.y ?? 0}%`,
        width: `${transform?.width ?? 100}%`,
        height: `${transform?.height ?? 100}%`,
        zIndex: transform?.zIndex ?? 1,
        transform: `rotate(${transform?.rotation ?? 0}deg) scale(${transform?.scale ?? 1})`,
        objectFit: 'cover',
        pointerEvents: 'none',
    };

    return <img src={url} style={style} className={className} alt="" />;
};

export const BackgroundLayer = ({ imageId, transform }: { imageId: any, transform: any }) => {
    const actualId = typeof imageId === 'object' ? imageId?.imageId : imageId;

    return (
        <Layer
            id={actualId}
            transform={transform}
            className={css({ filter: 'brightness(0.9)', transition: '0.3s' })}
        />
    );
};

export const CharacterLayer = ({ characterId, stateId, transform }: any) => {
    const resourceId = stateId || characterId;

    return (
        <Layer
            id={resourceId}
            transform={transform}
        />
    );
};
