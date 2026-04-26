import {css} from '../../../styled-system/css'
import Selector from "./Selector.tsx";
import MenuTextarea from "./MenuTextarea.tsx";

interface BlockMenuProps {
    title: string;
}
export default function BlockMenu({title}: BlockMenuProps) {
    const characterOptions = [
        { value: 'fon', label: 'fon' },
        { value: 'fonk', label: 'fonk' },
        { value: 'fenk', label: 'fenk' },
    ];

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
                {title}
            </div>
            <div className={css({
                display: 'flex',
                flexDirection: 'column',
                gap:'20px',
                alignItems: 'center',
                justifyContent: 'center',
            })}>
                <Selector title={'Персонаж'} options={characterOptions}></Selector>
                <MenuTextarea title={'Текст'}></MenuTextarea>
            </div>
        </div>
    )
}
