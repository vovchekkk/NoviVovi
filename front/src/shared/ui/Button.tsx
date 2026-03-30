import { cva } from '../../../styled-system/css'
import type { RecipeVariantProps } from '../../../styled-system/css'
import type { ComponentProps } from 'react'

const button = cva({
    base: {
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        fontWeight: '600',
        fontSize: '17px',
        lineHeight: '1.2',
        cursor: 'pointer',
        transition: 'all 0.25s cubic-bezier(0.4, 0, 0.2, 1)',
        border: 'none',
        whiteSpace: 'nowrap',
        userSelect: 'none',
        width: '100%',
        maxWidth: '320px',
    },

    variants: {
        variant: {
            primary: {
                backgroundColor: 'white',
                color: 'black',
                borderRadius: 'xl',
                boxShadow: 'xl',
                _hover: {
                    boxShadow: '2xl',
                    backgroundColor: 'gray.50',
                },
            },
        },
        disabled: {
            true: {
                opacity: 0.5,
                cursor: 'not-allowed',
                _hover: {
                    backgroundColor: 'white',
                    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
                },
                _active: {
                    backgroundColor: 'white',
                    transform: 'none',
                    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
                },
            },
            false: {},
        },
    },

    defaultVariants: {
        variant: 'primary',
    },
})

export type ButtonVariants = RecipeVariantProps<typeof button>

type ButtonProps = Omit<ComponentProps<'button'>, 'disabled'> & ButtonVariants & {
    children: React.ReactNode
    leftIcon?: React.ReactNode
    rightIcon?: React.ReactNode
}

export function Button({
                           variant = 'primary',   // явно задаём дефолт
                           disabled,
                           leftIcon,
                           rightIcon,
                           children,
                           ...props
                       }: ButtonProps) {
    return (
        <button
            className={button({ variant, disabled: !!disabled })}
            disabled={disabled}
            {...props}
        >
            {leftIcon && <span style={{ marginRight: '10px' }}>{leftIcon}</span>}
            {children}
            {rightIcon && <span style={{ marginLeft: '10px' }}>{rightIcon}</span>}
        </button>
    )
}