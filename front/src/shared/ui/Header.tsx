import {css} from '../../../styled-system/css'
import { Link } from 'react-router-dom'

interface HeaderProps {
    active?: 'main' | 'editor' | 'novels'
}
export default function Header({ active = 'main' }: HeaderProps) {
    return (
        <header className={css({
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            zIndex: 1000,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            px: '8',
            py: '5',
            bg: 'background',
            borderBottom: '1px solid token(colors.border)',
            boxShadow: '0 1px 0 token(colors.border-light)',
        })}>
            <nav className={css({
                display: 'flex',
                gap: '40px',
                fontSize: '17px',
                fontWeight: '500'
            })}>

                <Link
                    to="/"
                    className={css({
                        color: active === 'main' ? 'text' : 'text-secondary',
                        position: 'relative',
                        pb: '6px',
                        _after: active === 'main' ? {
                            content: '""',
                            position: 'absolute',
                            bottom: '-2px',
                            left: 0,
                            width: '100%',
                            height: '3px',
                            backgroundColor: 'card',
                        } : {}
                    })}
                >
                    Главная
                </Link>

                <Link
                    to="/editor"
                    className={css({
                        color: active === 'editor' ? 'text' : 'text-secondary',
                        position: 'relative',
                        pb: '6px',
                        _after: active === 'editor' ? {
                            content: '""',
                            position: 'absolute',
                            bottom: '-2px',
                            left: 0,
                            width: '100%',
                            height: '3px',
                            backgroundColor: 'card',
                        } : {}
                    })}
                >
                    Редактор
                </Link>

                <Link
                    to="/novels"
                    className={css({
                        color: active === 'novels' ? 'text' : 'text-secondary',
                        position: 'relative',
                        pb: '6px',
                        _after: active === 'novels' ? {
                            content: '""',
                            position: 'absolute',
                            bottom: '-2px',
                            left: 0,
                            width: '100%',
                            height: '3px',
                            backgroundColor: 'card',
                        } : {}
                    })}
                >
                    Новеллы
                </Link>
            </nav>
        </header>
    )
}