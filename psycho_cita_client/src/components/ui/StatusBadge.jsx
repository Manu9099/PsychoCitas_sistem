function getBadgeClasses(status) {
  switch (status) {
    case 'confirmada':
    case 'completada':
    case 'activo':
    case 'success':
      return 'bg-emerald-50 text-emerald-700 border border-emerald-200'

    case 'pendiente':
    case 'warning':
      return 'bg-amber-50 text-amber-700 border border-amber-200'

    case 'cancelada':
    case 'inactivo':
    case 'error':
      return 'bg-rose-50 text-rose-700 border border-rose-200'

    case 'no-asistio':
      return 'bg-slate-100 text-slate-700 border border-slate-200'

    default:
      return 'bg-slate-50 text-slate-700 border border-slate-200'
  }
}

export default function StatusBadge({ status = '', children, className = '' }) {
  const normalized = String(status).trim().toLowerCase()
  const label = children || status || 'Sin estado'

  return (
    <span
      className={[
        'inline-flex items-center rounded-full px-3 py-1 text-xs font-medium',
        getBadgeClasses(normalized),
        className,
      ].join(' ')}
    >
      {label}
    </span>
  )
}