import {useMemo} from "react";

export const useSceneSnapshot = (steps: any[], currentIndex: number | null) => {
    return useMemo(() => {
        if (!steps || steps.length === 0) {
            return { background: null, characters: [] };
        }

        const index = currentIndex !== null ? currentIndex : steps.length - 1;
        // Берем шаги ДО текущего (не включая текущий, если мы хотим "предпросмотр поверх")
        // Либо ВКЛЮЧАЯ текущий, если это просто плеер.
        // Для редактора лучше остановиться на index, а текущий шаг рендерить отдельно.
        const snapshotSteps = steps.slice(0, index + 1);

        let background: any = null;
        const charactersMap: Record<string, any> = {};


        for (const step of snapshotSteps) {
            const state = step.state || step;

            if (step.type === 'background') {
                const data = step.state || step;
                const id = data.background?.imageId || data.imageId;

                if (id) {
                    background = {
                        imageId: id,
                        transform: data.background?.transform || data.transform || {}
                    };
                }
            }

            if (step.type === 'show' && state.characterId && state.characterStateId) {
                charactersMap[state.characterId] = {
                    characterId: state.characterId,
                    characterStateId: state.characterStateId,
                    transform: state.transform || { x: 40, y: 30, width: 25, height: 70, scale: 1, rotation: 0, zIndex: 20 }
                };
            }

            if (step.type === 'hide' && state.characterId) {
                delete charactersMap[state.characterId];
            }
        }

        return {
            background,
            characters: Object.values(charactersMap)
        };
    }, [steps, currentIndex]);
};
