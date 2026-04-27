import { css } from '../../../styled-system/css';
import { BackgroundLayer, CharacterLayer } from "./Layer";
import { useSceneSnapshot } from "./SceneSnapshot.tsx";
import { useWatch } from "react-hook-form";

interface PreviewProps {
    steps: any[];
    selectedStepIndex: number | null;
    control?: any;
}

export default function Preview({ steps, selectedStepIndex, control }: PreviewProps) {
    const historyIndex = selectedStepIndex !== null ? selectedStepIndex - 1 : null;
    const { background: hBackground, characters: hCharacters } = useSceneSnapshot(steps, historyIndex);

    const watched = useWatch({
        control,
        name: ["type", "characterId", "characterStateId", "transform", "imageId", "background"] as const,
    });

    const [wType, wCharId, wStateId, wTransform, wImageId, wBackground] = watched;

    // Логика текущего шага
    const isEditingShow = wType === 'show' && !!wCharId && !!wStateId;
    const isEditingHide = wType === 'hide' && !!wCharId;
    const isEditingBG = wType === 'background' && (!!wImageId || !!wBackground?.imageId);

    const activeBackground = isEditingBG
        ? { imageId: wImageId || wBackground?.imageId, transform: wTransform || wBackground?.transform }
        : hBackground;

    let activeCharacters = [...hCharacters];

    if (isEditingShow) {
        activeCharacters = activeCharacters.filter(c => c.characterId !== wCharId);
        activeCharacters.push({
            characterId: wCharId,
            characterStateId: wStateId,
            transform: wTransform
        });
    }

    if (isEditingHide) {
        activeCharacters = activeCharacters.filter(c => c.characterId !== wCharId);
    }

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
            <div className={css({ position: 'absolute', inset: 0, backgroundImage: '...', pointerEvents: 'none', zIndex: 1 })} />
            {activeBackground?.imageId && (
                <BackgroundLayer
                    imageId={activeBackground.imageId}
                    transform={activeBackground.transform}
                />
            )}
            {activeCharacters
                .filter((char: any) => !!char.characterId && !!char.characterStateId)
                .sort((a: any, b: any) => (a.transform?.zIndex || 10) - (b.transform?.zIndex || 10))
                .map((char: any, index: number) => (
                    <CharacterLayer
                        key={char.characterId || `char-${index}`}
                        characterId={char.characterId}
                        stateId={char.characterStateId}
                        transform={char.transform}
                    />
                ))}

            {!activeBackground?.imageId && (
                <div className={css({
                    position: 'absolute', inset: 0,
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    color: '#888', fontSize: '18px'
                })}>
                    Нет фона
                </div>
            )}
        </div>
    );
}
