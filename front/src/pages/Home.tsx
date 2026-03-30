import { css } from '../../styled-system/css'
import Header from '../shared/ui/Header'
import VideoContainer from '../shared/ui/VideoContainer'
import CTAButton from '../shared/ui/CTAButton'

export default function Home() {
    return (
        <div className={css({
            bg: 'background',
            minHeight: '100vh',
            color: 'white',
        })}>
            <Header active="main" />

            <main className={css({
                pt: '90px',
                pb: '120px',
                px: '20px',
            })}>
                <VideoContainer />

                <CTAButton />
            </main>
        </div>
    )
}