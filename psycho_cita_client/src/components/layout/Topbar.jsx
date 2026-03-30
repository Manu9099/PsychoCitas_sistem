import { useLocation } from 'react-router-dom'
import { Menu, Bell } from 'lucide-react'
import { useAuthStore } from '../../app/authStore'

const pageTitles = {
  '/app': 'Dashboard',
  '/app/agenda': 'Agenda',
  '/app/pacientes': 'Pacientes',
}

export default function Topbar({ onOpenSidebar = () => {} }) {
  const location = useLocation()
  const user = useAuthStore((s) => s.user)

  const currentTitle = pageTitles[location.pathname] || 'PsychoCitas'

  return (
    <header className="sticky top-0 z-20 border-b border-slate-200 bg-white/90 backdrop-blur">
      <div className="flex items-center justify-between px-4 py-4 md:px-6">
        <div className="flex items-center gap-3">
          <button
            onClick={onOpenSidebar}
            className="inline-flex h-10 w-10 items-center justify-center rounded-2xl border border-slate-200 text-slate-600 lg:hidden"
          >
            <Menu className="h-5 w-5" />
          </button>

          <div>
            <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
              Panel
            </p>
            <h1 className="text-lg font-semibold tracking-tight text-slate-900">
              {currentTitle}
            </h1>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <button className="inline-flex h-10 w-10 items-center justify-center rounded-2xl border border-slate-200 text-slate-600 transition hover:bg-slate-50">
            <Bell className="h-4 w-4" />
          </button>

          <div className="hidden rounded-2xl border border-slate-200 bg-slate-50 px-3 py-2 md:block">
            <p className="text-xs text-slate-500">Bienvenido</p>
            <p className="text-sm font-medium text-slate-900">
              {user?.nombre || user?.email || 'Usuario'}
            </p>
          </div>
        </div>
      </div>
    </header>
  )
}