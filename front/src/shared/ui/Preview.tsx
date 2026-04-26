import {css} from '../../../styled-system/css'
import {useWatch} from "react-hook-form";
import {BackgroundLayer, CharacterLayer} from "./Layer.tsx";

interface PreviewProps {
    control:any;
}

type Url = {
    url:string;
}

export default function Preview({ control }: PreviewProps) {
    const background = useWatch({ control, name: "background" });

    const charactersFromRoot = useWatch({ control, name: "characters" });
    const charactersFromState = useWatch({ control, name: "state.characters" });

    const characters = charactersFromRoot?.length
        ? charactersFromRoot
        : charactersFromState || [];

    const [watchedType, watchedCharacterId, watchedCharacterStateId, watchedTransform] = useWatch({
        control,
        name: ["type", "characterId", "characterStateId", "transform"] as const,
    });

    const isShowStep = watchedType === 'show' && !!watchedCharacterId;

    return (
        <div className={css({
            width: '100%',
            maxWidth: '800px',
            margin: '0 auto',
            aspectRatio: '16 / 9',
            position: 'relative',
            backgroundColor: '#111',
            borderRadius: '12px',
            overflow: 'hidden',
            border: '3px solid #705661',
            boxShadow: '0 8px 30px rgba(0,0,0,0.4)'
        })}>
            <div className={css({
                position: 'absolute',
                inset: 0,
                backgroundImage: 'linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px)',
                backgroundSize: '40px 40px',
                pointerEvents: 'none'
            })} />

            {background?.imageId && (
                <BackgroundLayer
                    imageId={background.imageId}
                    transform={background.transform || {
                        x: 0, y: 0, width: 100, height: 100, scale: 1, rotation: 0, zIndex: 0
                    }}
                />
            )}

            {characters
                .filter((char: any) => char?.characterId)
                .sort((a: any, b: any) => (a.transform?.zIndex || 10) - (b.transform?.zIndex || 10))
                .map((char: any, index: number) => (
                    <CharacterLayer
                        key={char.characterId || `char-${index}`}
                        characterId={char.characterId}
                        stateId={char.characterStateId}
                        transform={char.transform}
                    />
                ))}

            {isShowStep && watchedCharacterId && (
                <CharacterLayer
                    characterId={watchedCharacterId}
                    stateId={watchedCharacterStateId}
                    transform={watchedTransform || {
                        x: 45, y: 30, width: 28, height: 68,
                        scale: 1, rotation: 0, zIndex: 30
                    }}
                />
            )}

            {!background?.imageId && (
                <div className={css({
                    position: 'absolute',
                    inset: 0,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    color: '#888',
                    fontSize: '18px',
                    textAlign: 'center',
                    padding: '20px'
                })}>
                    Выберите фон в форме редактирования
                </div>
            )}
        </div>
    );
}