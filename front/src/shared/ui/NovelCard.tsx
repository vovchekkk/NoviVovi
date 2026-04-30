import {css} from '../../../styled-system/css'

interface NovelCardProps {
    title: string
    cover: React.ReactNode;
    description: string
}

export default function NovelCard({
                                      title,
                                      description,
                                      cover,
                                  }: NovelCardProps) {
    return (
        <div className={css({
            display: 'flex',
            gap: '40px',
            height: '300px',
            alignItems: 'center',
            flexDirection: {base: 'column', md: 'row'},
        })}>

            <div className={css({
                width: '531px', // Ширина высчитана под 16:9 (200px * 1.77)
                height: '100%',
                flexShrink: 0
            })}>
                {cover}
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