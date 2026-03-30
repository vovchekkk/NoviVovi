import {css} from '../../../styled-system/css'

interface BlockMenuProps {
    title: string;
}
export default function BlockMenu({title}: BlockMenuProps) {
    return (
        <div className={css({
            backgroundColor: '#DFC6D1',
            color: 'black',
            flex: 1,
            minWidth: '400px',
            borderRadius:'12px',
        })}>
            <div className={css({
                margin: '20px',
                borderBottom: '1px solid black',
            })}>
                {title}
            </div>
        </div>
    )
}