import { css } from '../../../styled-system/css'
interface CTAButtonProps {
    onClick?: () => void;
}
export default function CTAButton({onClick}: CTAButtonProps) {
    return (
        <button
            onClick={onClick}
            className={css({
                display: 'block',
                margin: '80px auto 0',
                px: '52px',
                py: '20px',
                bg: 'transparent',
                color: 'white',                    // тёмный текст для контраста
                border: '3px solid #FADCD4',
                borderRadius: '9999px',
                fontSize: '19px',
                fontWeight: '600',
                cursor: 'pointer',
                transition: 'all 0.35s ease',
                boxShadow: 'glow',
                _hover: {
                    bg: 'card',
                    color: 'background',
                    borderColor: 'card',
                    transform: 'translateY(-4px)',
                    boxShadow: 'glow-hover',
                }
            })}
        >
            Создай свою новую новеллу
        </button>
    )
}