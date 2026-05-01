import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css'
import AssetsContainer from "../shared/ui/AssetsContainer.tsx";
import { useEffect } from "react";
import {useParams} from "react-router-dom";
import { setLastNovelId } from "../shared/lib/novelSession.ts";
export default function Assets() {
    const {novelId} = useParams();

    useEffect(() => {
        if (novelId) {
            setLastNovelId(novelId);
        }
    }, [novelId]);

    return (
        <div className={css({
            bg: '#775D68',
            height: '100vh',
            display: 'flex',
            overflow: 'hidden',
            flexDirection: 'column',
            color: 'text',
        })}>
            <Header active="editor" />
            <main className={css({
                flex:1,
                minHeight: 0,
                pt: '90px',
                pb: '0px',
                px: '0px',
            })}>
                <AssetsContainer novelId={novelId}></AssetsContainer>
            </main>
        </div>
    )
}
