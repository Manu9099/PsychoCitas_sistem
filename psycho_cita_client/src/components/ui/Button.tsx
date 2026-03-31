import React from 'react'

type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger'
type ButtonSize = 'sm' | 'md' | 'lg'

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode
  variant?: ButtonVariant
  size?: ButtonSize
  loading?: boolean
  className?: string
}

function getVariantClasses(variant: ButtonVariant) {
  switch (variant) {
    case 'secondary':
      return 'bg-white text-slate-900 border border-slate-300 hover:bg-slate-50'
    case 'ghost':
      return 'bg-transparent text-slate-700 hover:bg-slate-100'
    case 'danger':
      return 'bg-rose-600 text-white hover:bg-rose-700'
    default:
      return 'bg-slate-900 text-white hover:opacity-90'
  }
}

function getSizeClasses(size: ButtonSize) {
  switch (size) {
    case 'sm':
      return 'h-9 px-3 text-sm rounded-xl'
    case 'lg':
      return 'h-12 px-5 text-base rounded-2xl'
    default:
      return 'h-10 px-4 text-sm rounded-2xl'
  }
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      children,
      className = '',
      variant = 'primary',
      size = 'md',
      loading = false,
      disabled = false,
      type = 'button',
      ...props
    },
    ref
  ) => {
    const isDisabled = disabled || loading

    return (
      <button
        ref={ref}
        type={type}
        disabled={isDisabled}
        className={[
          'inline-flex items-center justify-center gap-2 font-medium transition outline-none',
          'focus-visible:ring-2 focus-visible:ring-slate-300',
          'disabled:cursor-not-allowed disabled:opacity-60',
          getVariantClasses(variant),
          getSizeClasses(size),
          className,
        ].join(' ')}
        {...props}
      >
        {loading ? 'Cargando...' : children}
      </button>
    )
  }
)

Button.displayName = 'Button'

export default Button