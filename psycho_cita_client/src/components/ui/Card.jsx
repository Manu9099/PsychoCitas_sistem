export function Card({ children, className = '' }) {
  return (
    <div
      className={[
        'rounded-3xl border border-slate-200 bg-white shadow-sm',
        className,
      ].join(' ')}
    >
      {children}
    </div>
  )
}

export function CardHeader({ children, className = '' }) {
  return (
    <div className={['border-b border-slate-100 p-5', className].join(' ')}>
      {children}
    </div>
  )
}

export function CardTitle({ children, className = '' }) {
  return (
    <h3 className={['text-lg font-semibold tracking-tight', className].join(' ')}>
      {children}
    </h3>
  )
}

export function CardDescription({ children, className = '' }) {
  return (
    <p className={['mt-1 text-sm text-slate-500', className].join(' ')}>
      {children}
    </p>
  )
}

export function CardContent({ children, className = '' }) {
  return <div className={['p-5', className].join(' ')}>{children}</div>
}

export function CardFooter({ children, className = '' }) {
  return (
    <div className={['border-t border-slate-100 p-5', className].join(' ')}>
      {children}
    </div>
  )
}