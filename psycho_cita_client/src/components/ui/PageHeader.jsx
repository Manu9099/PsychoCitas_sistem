export default function PageHeader({
  eyebrow,
  title,
  description,
  actions,
  className = '',
}) {
  return (
    <div
      className={[
        'flex flex-col gap-4 md:flex-row md:items-end md:justify-between',
        className,
      ].join(' ')}
    >
      <div>
        {eyebrow ? (
          <p className="mb-2 text-sm font-medium text-violet-600">{eyebrow}</p>
        ) : null}

        <h1 className="text-2xl font-semibold tracking-tight md:text-3xl">
          {title}
        </h1>

        {description ? (
          <p className="mt-2 max-w-2xl text-sm text-slate-500 md:text-base">
            {description}
          </p>
        ) : null}
      </div>

      {actions ? <div className="flex items-center gap-3">{actions}</div> : null}
    </div>
  )
}