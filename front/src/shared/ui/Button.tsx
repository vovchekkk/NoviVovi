import {css} from '../../../styled-system/css'

interface ButtonProps {
    word?: string;
}
export default function Button({word}: ButtonProps) {
    return (
        <div className={css({
            width:'20%',
            height:'auto',
            padding:'10px',
            margin: '10px',
            backgroundColor: 'white',
            borderRadius: '10px',
            border: '1px black solid',
            textAlign: 'center',
            _hover: {
                bg: '#DBDBDB',
            }
        })}>
            {word}
        </div>
    )
}
