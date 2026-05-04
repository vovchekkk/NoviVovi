import { css } from '../../../styled-system/css'
import promoVideo from '../../assets/video.mp4'
export default function VideoContainer() {
    return (
        <div className={css({
            width: '100%',
            maxWidth: '1180px',
            margin: '0 auto',
            position: 'relative',
            borderRadius: '24px',
            overflow: 'hidden',
            backgroundColor: 'card',
            boxShadow: '0 10px 40px rgba(0, 0, 0, 0.45)',
            // aspectRatio удаляем!
        })}>

            <video
                autoPlay
                muted
                loop
                playsInline
                className={css({
                    width: '100%', // Оставляем только ширину
                    height: 'auto', // Высота подстроится сама
                    display: 'block',
                })}
            >
                <source
                    src={promoVideo}
                    type="video/mp4"
                />
                Ваш браузер не поддерживает видео.
            </video>

            <div className={css({
                position: 'absolute',
                insetX: 0,
                top: 0,
                height: '120px',
                pointerEvents: 'none',
            })} />

            <div className={css({
                position: 'absolute',
                insetX: 0,
                bottom: 0,
                height: '140px',
                pointerEvents: 'none',
            })} />
        </div>
    )
}