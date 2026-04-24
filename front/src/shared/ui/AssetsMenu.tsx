import {css} from '../../../styled-system/css'
import AssetsBlock from "./AssetsBlock.tsx";

export default function AssetsMenu() {
    return (
        <div className={css({
            backgroundColor: '#DFC6D1',
            color: 'black',
            flex: 1,
            minWidth: '400px',
            borderRadius:'12px',
        })}>
            <div className={css({
                fontSize: '20px',
                margin: '20px',
                borderBottom: '1px solid black',
            })}>
                Персонажи
            </div>
            <div className={css({
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
            })}>
                <AssetsBlock title={'Анна'}></AssetsBlock>
                <AssetsBlock title={'Мария'}></AssetsBlock>
                <AssetsBlock title={'Борис'}></AssetsBlock>
            </div>
        </div>
    )
}