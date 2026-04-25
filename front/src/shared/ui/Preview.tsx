import {css} from '../../../styled-system/css'
import {useWatch} from "react-hook-form";
import {useEffect, useState} from "react";
import api from "../../api.tsx";
import bg from "../../assets/back.jpg"
import {BackgroundLayer, CharacterLayer} from "./Layer.tsx";

interface PreviewProps {
    image: string;
    control:any;
}

type Url = {
    url:string;
}

export default function Preview({image, control}: PreviewProps) {
    const [imageUrl, setImageUrl] = useState<string>(bg);
    const [isLoading, setIsLoading] = useState(false);
    const transform = useWatch({
        control,
        name: 'transform',
        defaultValue: { x: 0, y: 0, width: 100, height: 100, scale: 1, rotation: 0, zIndex: 1 }
    });

    const background = useWatch({
        control,
        name: 'background'
    });

    const imageId = useWatch({
        control,
        name: 'background.imageId'
    });
    const characters = useWatch({ control, name: 'characters' });

    const currentImageId = useWatch({ control, name: 'imageId' });
    const currentTransform = useWatch({ control, name: 'transform' });
    useEffect(() => {
        if (!imageId) {
            setImageUrl(bg);
            return;
        }

        const fetchImageUrl = async () => {
            setIsLoading(true);
            try {
                const {data} = await api.get<Url>(`/images/${imageId}`);
                setImageUrl(data.url);
                console.log(imageId);
            } catch (error) {
                console.error("Ошибка при получении URL:", error);
                setImageUrl(bg);
            } finally {
                setIsLoading(false);
            }
        };

        fetchImageUrl();
    }, [imageId, image]);

    const imageStyle: React.CSSProperties = {
        position: 'absolute',
        left: `${transform?.x ?? 0}%`,
        top: `${transform?.y ?? 0}%`,
        width: `${transform?.width ?? 100}%`,
        height: `${transform?.height ?? 100}%`,
        zIndex: transform?.zIndex ?? 1,
        transform: `rotate(${transform?.rotation ?? 0}deg) scale(${transform?.scale ?? 1})`,
        objectFit: 'cover',
        transition: 'all 0.1s linear',
    };

    return (
        <div className={css({
            width: '80%',
            margin: '0 auto',
            aspectRatio: '16 / 9',
            position: 'relative',
            backgroundColor: '#111',
            borderRadius: '12px',
            overflow: 'hidden',
            border: '2px solid #333',
            boxShadow: '0 4px 20px rgba(0,0,0,0.3)'
        })}>
            <div className={css({
                position: 'absolute',
                inset: 0,
                pointerEvents: 'none',
                border: '1px dashed rgba(255,255,255,0.2)'
            })} />

            {background?.imageId && (
                <BackgroundLayer
                    imageId={background.imageId}
                    transform={background.transform}
                />
            )}

            {characters?.map((char: any, index: number) => (
                <CharacterLayer
                    key={char.characterId || index}
                    characterId={char.characterId}
                    stateId={char.characterStateId}
                    transform={char.transform}
                />
            ))}
        </div>
    )
}