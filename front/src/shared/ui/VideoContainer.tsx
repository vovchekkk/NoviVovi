import { css } from '../../../styled-system/css'
export default function VideoContainer() {
    return (
        <div className={css({
            width: '100%',
            maxWidth: '1180px',
            margin: '0 auto',
            position: 'relative',
            aspectRatio: '16 / 9',
            borderRadius: '24px',
            overflow: 'hidden',
            backgroundColor: 'card',        // ← твой цвет
            boxShadow: '0 10px 40px rgba(0, 0, 0, 0.45)',
        })}>

            <video
                controls
                autoPlay
                muted
                loop
                playsInline
                className={css({
                    width: '100%',
                    height: '100%',
                    objectFit: 'cover',
                    display: 'block',
                })}
            >
                <source
                    src="https://assets.mixkit.co/videos/preview/754/754-large.mp4"
                    type="video/mp4"
                />
                Ваш браузер не поддерживает видео.
            </video>

            <div className={css({
                position: 'absolute',
                insetX: 0,
                top: 0,
                height: '120px',
                background: 'linear-gradient(to bottom, #1a1a1a 35%, transparent)',
                pointerEvents: 'none',
            })} />

            <div className={css({
                position: 'absolute',
                insetX: 0,
                bottom: 0,
                height: '140px',
                background: 'linear-gradient(to top, #1a1a1a 40%, transparent)',
                pointerEvents: 'none',
            })} />
        </div>
    )
}