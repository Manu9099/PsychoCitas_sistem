import { NavLink, useNavigate } from 'react-router-dom'
import {
  LayoutDashboard,
  CalendarDays,
  Users,
  CreditCard,
  LogOut,
  HeartHandshake,
  X,
} from 'lucide-react'
import { useAuthStore } from '../../app/authStore'

const navItems = [
  {
    to: '/app',
    label: 'Dashboard',
    icon: LayoutDashboard,
    end: true,
  },
  {
    to: '/app/agenda',
    label: 'Agenda',
    icon: CalendarDays,
  },
  {
    to: '/app/pacientes',
    label: 'Pacientes',
    icon: Users,
  },
  {
    to: '/app/pagos',
    label: 'Pagos',
    icon: CreditCard,
  },
]

function getNavClass(isActive) {
  return [
    'flex items-center gap-3 rounded-2xl px-3 py-3 text-sm font-medium transition',
    isActive
      ? 'bg-slate-900 text-white shadow-sm'
      : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900',
  ].join(' ')
}

export default function Sidebar({ mobileOpen = false, onClose = () => {} }) {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const logout = useAuthStore((s) => s.logout)

  const handleLogout = () => {
    logout()
    onClose()
    navigate('/login')
  }

  return (
    <>
      {mobileOpen ? (
        <button
          aria-label="Cerrar menú"
          onClick={onClose}
          className="fixed inset-0 z-30 bg-slate-900/30 lg:hidden"
        />
      ) : null}

      <aside
        className={[
          'fixed inset-y-0 left-0 z-40 flex w-72 flex-col border-r border-slate-200 bg-white transition-transform duration-200',
          mobileOpen ? 'translate-x-0' : '-translate-x-full',
          'lg:static lg:z-auto lg:w-65 lg:translate-x-0',
        ].join(' ')}
      >
        <div className="flex items-center justify-between border-b border-slate-100 p-6">
          <div className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-violet-100 text-violet-700">
              <HeartHandshake className="h-5 w-5" />
            </div>

            <div>
              <p className="text-sm font-semibold text-slate-900">PsychoCitas</p>
              <p className="text-xs text-slate-500">Panel interno</p>
            </div>
          </div>

          <button
            onClick={onClose}
            className="inline-flex h-10 w-10 items-center justify-center rounded-2xl border border-slate-200 text-slate-600 lg:hidden"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="flex-1 p-4">
          <p className="px-3 pb-2 text-xs font-medium uppercase tracking-wide text-slate-400">
            Navegación
          </p>

          <nav className="space-y-2">
            {navItems.map(({ to, label, icon: Icon, end }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                onClick={onClose}
                className={({ isActive }) => getNavClass(isActive)}
              >
                <Icon className="h-4 w-4" />
                <span>{label}</span>
              </NavLink>
            ))}
          </nav>
        </div>

        <div className="border-t border-slate-100 p-4">
          <div className="mb-4 rounded-2xl bg-slate-50 p-4">
            <p className="text-xs text-slate-500">Sesión activa</p>
            <p className="mt-1 text-sm font-medium text-slate-900">
              {user?.nombre || user?.email || 'Usuario'}
            </p>
            {user?.email ? (
              <p className="mt-1 text-xs text-slate-500">{user.email}</p>
            ) : null}
          </div>

          <button
            onClick={handleLogout}
            className="flex w-full items-center justify-center gap-2 rounded-2xl border border-slate-300 px-4 py-3 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
          >
            <LogOut className="h-4 w-4" />
            Cerrar sesión
          </button>
        </div>
      </aside>
    </>
  )
}