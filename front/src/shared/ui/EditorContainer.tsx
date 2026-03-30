import {css} from '../../../styled-system/css'
import EditorHeader from "./EditorHeader.tsx";

export default function EditorContainer() {
    return (
        <div className={css({
            minHeight: '100vh',
            background: '#775D68',
            display: 'flex',
            flexDirection: 'column',
            width: '100%',
            gap: '0px',
        })}>
            <div className={css({
                display: 'flex',
                minHeight: '0',
                flex:1,
                gap: '20px',
                width: '100%',
            })}>
                <div className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    width: '100%',
                    gap: '0px',
                })}>
                    <EditorHeader active="editor"/>
                    <div className={css({
                        backgroundColor: 'white',
                        color: 'black',
                        width: '100%',
                        flex: 4,
                    })}>
                        Редактор
                    </div>
                </div>
                <div className={css({
                    backgroundColor: '#DFC6D1',
                    color: 'black',
                    width: '20%',
                    flex: 1,
                    minWidth: '280px',
                    borderRadius:'12px',
                })}>
                    Боковое меню
                </div>
            </div>
        </div>
    )
}