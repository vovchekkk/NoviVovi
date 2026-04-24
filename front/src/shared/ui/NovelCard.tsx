import {css} from '../../../styled-system/css'

interface NovelCardProps {
    image: string
    title: string
    description: string
}

export default function NovelCard({
                                      image,
                                      title,
                                      description,
                                  }: NovelCardProps) {
    return (
        <div className={css({
            display: 'flex',
            gap: '40px',
            alignItems: 'center',
            flexDirection: {base: 'column', md: 'row'},
        })}>

            <div className={css({
                width: {base: '100%', md: '420px'},
                flexShrink: 0,
                borderRadius: 'card',
                overflow: 'hidden',
                boxShadow: 'card',
            })}>
                <img
                    src={image}
                    alt={title}
                    className={css({
                        width: '100%',
                        height: 'auto',
                        display: 'block',
                        aspectRatio: '16 / 9',
                        objectFit: 'cover',
                    })}
                />
            </div>

            <div className={css({
                flex: 1,
            })}>
                <h2 className={css({
                    fontSize: '26px',
                    fontWeight: '700',
                    lineHeight: '1.3',
                    mb: '12px',
                    color: 'card',
                })}>
                    {title}
                </h2>

                <p className={css({
                    fontSize: '17px',
                    lineHeight: '1.6',
                    color: 'text-secondary',
                    maxWidth: '520px',
                })}>
                    {description}
                </p>
            </div>
        </div>
    )
}