import { css } from '../../../styled-system/css';
import { BackgroundLayer, CharacterLayer } from "./Layer";
import { useSceneSnapshot } from "./SceneSnapshot.tsx";
import { useWatch } from "react-hook-form";

interface PreviewProps {
    labelId: string;
    stepId: string | null;
    control?: any;
    novelId: string;
    characterOptions: any[];
}

export default function Preview({ labelId, stepId, control, novelId, characterOptions }: PreviewProps) {
    const getActualImageId = (charId: string, stateId: string) => {
        const character = characterOptions?.find(c => c.id === charId);
        const state = character?.states?.find((s: any) => s.id === stateId);
        return state?.imageId;
    };

    const { background: hBackground, characters: hCharacters, replica, menu } = useSceneSnapshot(novelId, labelId, stepId);
    const speaker = characterOptions?.find(c => c.id === replica?.speakerId);
    const speakerName = speaker?.name || "";
    const speakerColor = speaker?.nameColor || "#ffffff";
    const watched = useWatch({
        control,
        name: ["type", "characterId", "characterStateId", "characterTransform", "transform", "imageId"] as const,
    });

    const [wType, wCharId, wStateId, wCharacterTransform, wTransform, wImageId] = watched;

    const isEditingShow = wType === 'show_character' && !!wCharId && !!wStateId;
    const isEditingHide = wType === 'hide_character' && !!wCharId;
    const isEditingBG = wType === 'show_background' && !!wImageId;

    const activeBackground = isEditingBG
        ? {
            imageId: wImageId,
            transform: wTransform
        }
        : hBackground;

    let activeCharacters = [...hCharacters];

    if (isEditingShow) {
        activeCharacters = activeCharacters.filter(c => c.characterId !== wCharId);
        activeCharacters.push({
            characterId: wCharId,
            characterStateId: wStateId,
            transform: wCharacterTransform
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
                    novelId={novelId}
                />
            )}
            {activeCharacters
                .filter((char: any) => !!char.characterId && !!char.characterStateId)
                .sort((a: any, b: any) => (a.transform?.zIndex || 10) - (b.transform?.zIndex || 10))
                .map((char: any, index: number) => {
                    const resolvedImageId = getActualImageId(char.characterId, char.characterStateId);

                    if (!resolvedImageId) return null;

                    return (
                        <CharacterLayer
                            key={char.characterId || `char-${index}`}
                            imageId={resolvedImageId}
                            transform={char.transform}
                            novelId={novelId}
                        />
                    );
                })}
            {replica && replica.text && (
                <div className={css({
                    position: 'absolute',
                    bottom: 0,
                    left: 0,
                    width: '100%',
                    height: '25%',
                    minHeight: '100px',
                    background: 'linear-gradient(to right, transparent 0%, rgba(0,0,0,0.7) 15%, rgba(0,0,0,0.7) 85%, transparent 100%)',
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'flex-start',
                    justifyContent: 'flex-start',
                    paddingLeft: '20%',
                    paddingRight: '5%',
                    zIndex: 100,
                })}>
                    <div className={css({
                        fontSize: '24px',
                        marginBottom: '4px',
                        textShadow: '1px 1px 2px rgba(0,0,0,0.8)'
                    })}
                         style={{ color: speakerColor }}
                    >
                        {speakerName}
                    </div>

                    <div className={css({
                        fontSize: '20px',
                        marginLeft: '3%',
                        color: 'white',
                        textAlign: 'left',
                        maxWidth: '80%',
                        lineHeight: '1.2'
                    })}>
                        {replica.text}
                    </div>
                </div>
            )}

            {menu && menu.choices && menu.choices.length > 0 && (
                <div className={css({
                    position: 'absolute',
                    top: '35%',
                    left: '50%',
                    transform: 'translate(-50%, -50%)',
                    width: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '12px',
                    alignItems: 'center',
                    zIndex: 110,
                })}>
                    {menu.choices.map((choice: any, index: number) => (
                        <div
                            key={index}
                            className={css({
                                width: '100%',
                                height: '5%',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                background: 'linear-gradient(to right, transparent 0%, rgba(20, 20, 20, 0.85) 25%, rgba(20, 20, 20, 0.85) 75%, transparent 100%)',
                                transition: 'all 0.2s',
                            })}
                        >
                            <span className={css({
                                color: '#c2c2c2',
                                fontSize: '18px',
                                fontWeight: 'normal',
                                letterSpacing: '0.5px',
                                textShadow: '1px 1px 2px rgba(0,0,0,0.8)'
                            })}>
                            {choice.text}
                        </span>
                        </div>
                    ))}
                </div>
            )}

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
