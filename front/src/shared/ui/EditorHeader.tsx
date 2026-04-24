import {css} from '../../../styled-system/css'
import { Link } from 'react-router-dom'

interface EditorHeaderProps {
    active?: 'scenes' | 'editor' | 'assets'
}
export default function EditorHeader({ active  = 'editor' }: EditorHeaderProps) {
    return (
        <header className={css({
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
        })}>
            <nav className={css({
                display: 'flex',
                gap: '40px',
                fontSize: '20px',
                fontWeight: '500'
            })}>

                <Link
                    to="/editor"
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'editor' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Редактор
                </Link>

                <Link
                    to="/editor/scenes"
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'scenes' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Сцены
                </Link>

                <Link
                    to="/editor/assets"
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'assets' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Ассеты
                </Link>
            </nav>
        </header>
    )
}