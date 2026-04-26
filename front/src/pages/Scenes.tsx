import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css'
import EditorHeader from "../shared/ui/EditorHeader.tsx";
import InfiniteCanvas from "../shared/ui/InfiniteCanvas.tsx";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { setLastNovelId } from "../shared/lib/novelSession.ts";
export default function Scenes() {
    const { novelId } = useParams<{ novelId: string }>();

    useEffect(() => {
        if (novelId) {
            setLastNovelId(novelId);
        }
    }, [novelId]);

    return (
        <div className={css({
            bg: '#775D68',
            color: 'text',
        })}>
            <Header active="editor" />
            <main className={css({
                pt: '90px',
                pb: '0px',
                px: '0px',
            })}>
                <div className={css({
                    background: '#775D68',
                    display: 'flex',
                    flexDirection: 'column',
                    width: '100%',
                    height: 'calc(100vh - 90px)',
                })}>
                    <EditorHeader active="scenes"/>
                    <div className={css({
                        flex:1,
                        minHeight: '0',
                        backgroundColor: 'white',
                        color: 'black',
                        width: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                    })}>
                        {novelId ? (
                            <InfiniteCanvas novelId={novelId} />
                        ) : (
                            <div className={css({
                                flex: 1,
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '16px',
                            })}>
                                Не найден ID новеллы в URL.
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    )
}
