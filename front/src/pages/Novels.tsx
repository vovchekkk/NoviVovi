import Header from "../shared/ui/Header.tsx";
import { css } from '../../styled-system/css'
import NovelCard from "../shared/ui/NovelCard.tsx";

export default function Novels() {
    return (
        <div className={css({
            bg: 'background',
            minHeight: '100vh',
            color: 'text',
        })}>
            <Header active="novels" />

            <main className={css({
                pt: '90px',
                pb: '80px',
                px: { base: '20px', md: '40px' },
            })}>

                <div className={css({
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '60px',
                    maxWidth: 'content-max',
                    margin: '0 auto',
                })}>

                    <NovelCard
                        image="https://picsum.photos/id/1015/800/450"
                        title="Название супер крутой новеллы"
                        description="Суперское описание супер крутой новеллы"
                    />

                    <NovelCard
                        image="https://picsum.photos/id/1015/800/450"
                        title="Название супер крутой новеллы"
                        description="Суперское описание супер крутой новеллы"
                    />

                    <NovelCard
                        image="https://picsum.photos/id/1015/800/450"
                        title="Название супер крутой новеллы"
                        description="Суперское описание супер крутой новеллы"
                    />
                </div>
            </main>
        </div>
    )
}