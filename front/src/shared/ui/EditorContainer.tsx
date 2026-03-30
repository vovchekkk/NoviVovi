import {css} from '../../../styled-system/css'
import EditorHeader from "./EditorHeader.tsx";
import Preview from "./Preview.tsx";
import BlockPanel from "./BlockPanel.tsx";
import BlockMenu from "./BlockMenu.tsx";

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
                        padding: '20px',
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '20px',
                        flex: 4,
                    })}>
                        <Preview image="https://picsum.photos/id/1015/800/450"></Preview>
                        <BlockPanel></BlockPanel>
                    </div>
                </div>
                <BlockMenu title={'Фон'}></BlockMenu>
            </div>
        </div>
    )
}