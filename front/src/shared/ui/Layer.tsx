import { useEffect, useState } from "react";
import { css } from "../../../styled-system/css";
import api from "../../api";

interface LayerProps {
    id: string;
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
    const scaleValue = transform?.scale ?? 1;
    const rotateValue = transform?.rotation ?? 0;

    useEffect(() => {
        if (!id) return;

        api.get<{ url: string }>(`/images/${id}`)
            .then((res) => {
                setUrl(res.data.url);
            })
    }, [id]);

    if (!url)
        return null;

    const style: React.CSSProperties = {
        position: 'absolute',
        left: `${transform?.x ?? 0}%`,
        top: `${transform?.y ?? 0}%`,
        width: `${transform?.width ?? 100}%`,
        height: `${transform?.height ?? 100}%`,
        zIndex: transform?.zIndex ?? 1,
        transform: `rotate(${rotateValue}deg) scale(${scaleValue})`,
        objectFit: 'cover',
        pointerEvents: 'none',
        transition: 'all 0.1s linear',
    };

    return (
        <img
            src={url || ''}
            style={style}
            alt="Layer"
        />
    );
};

export const BackgroundLayer = ({ imageId, transform }: { imageId: string, transform: any }) => {
    return (
        <Layer
            id={imageId}
            transform={transform}
            className={css({ filter: 'brightness(0.9)' })}
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