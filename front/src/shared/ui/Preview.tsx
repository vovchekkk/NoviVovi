import {css} from '../../../styled-system/css'

interface PreviewProps {
    image: string;
}

export default function Preview({image}: PreviewProps) {
    return (
        <div className={css({})}>
            <img
                src={image}
                alt={'Preview'}
                className={css({
                    width: '70%',
                    display: 'block',
                    margin: '0 auto',
                    aspectRatio: '16 / 9',
                    objectFit: 'cover',
                    borderRadius: '12px',
                })}
            />
        </div>
    )
}