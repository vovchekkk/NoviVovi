import { useEffect, useState } from "react";
import { css } from "../../../styled-system/css";
import api from "../../api";

interface LayerProps {
    id: string | undefined | null;
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
    novelId: string;
}

export const Layer = ({ id, transform, className, novelId }: LayerProps) => {
    const [url, setUrl] = useState<string | null>(null);

    useEffect(() => {
        if (!id) {
            setUrl(null);
            return;
        }

        api.get<{ url: string }>(`novels/${novelId}/images/${id}`)
            .then((res) => {
                setUrl(res.data.url);
            })
            .catch(() => {
                setUrl(null);
            });

    }, [id, novelId]);

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

export const BackgroundLayer = ({ imageId, transform, novelId }: { imageId: any, transform: any, novelId:string }) => {
    const actualId = typeof imageId === 'object' ? imageId?.imageId : imageId;

    return (
        <Layer
            id={actualId}
            transform={transform}
            className={css({ filter: 'brightness(0.9)', transition: '0.3s' })}
            novelId={novelId}
        />
    );
};

export const CharacterLayer = ({ characterId, stateId, transform, novelId  }: any) => {
    const resourceId = stateId || characterId;

    return (
        <Layer
            id={resourceId}
            transform={transform}
            novelId={novelId}
        />
    );
};
