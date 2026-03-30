import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css'
import EditorContainer from "../shared/ui/EditorContainer.tsx";

export default function Editor() {
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
                px: '8px',
            })}>
                <EditorContainer></EditorContainer>
            </main>
        </div>
    )
}