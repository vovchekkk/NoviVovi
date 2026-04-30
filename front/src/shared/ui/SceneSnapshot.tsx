import { useState, useEffect } from "react";
import { charactersApi } from "../api/client";
import {formatTransformForFrontend} from "../../pages/Editor.tsx";

export const useSceneSnapshot = (novelId: string, labelId: string, stepId: string | null) => {
    const [data, setData] = useState<{
        background: any;
        characters: any[];
        replica: any;
        menu: any;
        loading: boolean;
    }>({
        background: null,
        characters: [],
        replica: null,
        menu: null,
        loading: false
    });

    useEffect(() => {
        if (!novelId || !labelId || !stepId) {
            setData({
                background: null,
                characters: [],
                replica: null,
                menu: null,
                loading: false
            });
            return;
        }

        const fetchSnapshot = async () => {
            setData(prev => ({ ...prev, loading: true }));
            try {
                const response = await charactersApi.getStepPreview(novelId, labelId, stepId);
                const result = response.data;

                setData({
                    background: result.background ? {
                        imageId: result.background.image?.id,
                        url: result.background.image?.url,
                        transform: formatTransformForFrontend(result.background.transform) || {}
                    } : null,
                    characters: (result.charactersOnScene || []).map((item: any) => ({
                        characterId: item.character?.id,
                        characterStateId: item.state?.id,
                        imageId: item.state?.image?.id,
                        url: item.state?.image?.url,
                        transform: formatTransformForFrontend(item.transform) || { x: 50, y: 50, width: 40, height: 40, scale: 1, rotation: 0, zIndex: 10 }
                    })),
                    replica: result.replica,
                    menu: result.menu,
                    loading: false
                });
            } catch (error) {
                console.error("Ошибка при получении превью сцены:", error);
                setData(prev => ({ ...prev, loading: false }));
            }
        };

        fetchSnapshot();
    }, [novelId, labelId, stepId]);

    return data;
};