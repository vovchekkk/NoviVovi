import {css} from '../../../styled-system/css'
import Button from "./Button.tsx";
import Block from "./Block.tsx";

export default function BlockPanel() {
    return (
        <div className={css({
            width:'70%',
            minHeight: '10em',
            height: 'auto',
            margin: '0 auto',
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#DFC6D1',
            borderRadius: '12px',
        })}>
            <Button word={'Добавить блок'}></Button>
            <div className={css({
                minHeight: '10em',
                height: 'auto',
                display: 'flex',
                flexDirection: 'column',
            })}>
                <Block number={1} title={'Фон'}></Block>
                <Block number={2} title={'Появление'}></Block>
                <Block number={3} title={'Выбор'}></Block>
                <Block number={4} title={'Переход'}></Block>
            </div>
        </div>
    )
}