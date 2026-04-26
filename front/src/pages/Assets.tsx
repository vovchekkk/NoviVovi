import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css'
import AssetsContainer from "../shared/ui/AssetsContainer.tsx";
import {useParams} from "react-router-dom";
export default function Assets() {
    const {novelId} = useParams();
    return (
        <div className={css({
            bg: '#775D68',
            minHeight: '100vh',
            color: 'text',
        })}>
            <Header active="editor" />
            <main className={css({
                pt: '90px',
                pb: '0px',
                px: '0px',
            })}>
                <AssetsContainer novelId={novelId}></AssetsContainer>
            </main>
        </div>
    )
}