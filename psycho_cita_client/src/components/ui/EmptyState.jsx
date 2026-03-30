import Button from './Button'

export default function EmptyState({
  title = 'Sin resultados',
  description = 'Todavía no hay información para mostrar.',
  actionLabel,
  onAction,
  className = '',
}) {
  return (
    <div
      className={[
        'flex flex-col items-center justify-center rounded-3xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center',
        className,
      ].join(' ')}
    >
      <div className="max-w-md">
        <h3 className="text-lg font-semibold tracking-tight text-slate-900">
          {title}
        </h3>
        <p className="mt-2 text-sm leading-6 text-slate-500">{description}</p>

        {actionLabel && onAction ? (
          <div className="mt-6">
            <Button variant="secondary" onClick={onAction}>
              {actionLabel}
            </Button>
          </div>
        ) : null}
      </div>
    </div>
  )
}