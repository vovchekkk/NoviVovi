import {css} from '../../../styled-system/css'

interface BlockProps {
    title: string;
    number: number | string;
}

export default function Block({title, number}: BlockProps) {
    return (

        <div className={css({
            height: '40%',
            display: 'flex',
            alignItems: 'stretch',
            padding: '4px',
            margin: '5px',
            backgroundColor: '#F8EDEB',
            borderRadius: '12px',
        })}>
            <div className={css({
                backgroundColor: '#705661',
                color: 'white',
                width: '52px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: '12px',
                flexShrink: 0,
                marginRight: '5px',
            })}>
                {number}
            </div>
            <div className={css({
                padding: '10px',
                backgroundColor: 'white',
                borderRadius: '12px',
                flex:1,
            })}>
                {title}
            </div>
        </div>
    )
}