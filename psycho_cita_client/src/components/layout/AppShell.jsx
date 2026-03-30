import { Link, Outlet } from 'react-router-dom'

export function AppShell() {
  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <div className="grid min-h-screen lg:grid-cols-[240px_1fr]">
        <aside className="border-r border-slate-200 bg-white p-6">
          <p className="text-sm font-semibold text-violet-600">PsychoCitas</p>
          <nav className="mt-6 flex flex-col gap-2">
            <Link className="rounded-xl px-3 py-2 hover:bg-slate-100" to="/app">
              Dashboard
            </Link>
            <Link className="rounded-xl px-3 py-2 hover:bg-slate-100" to="/app/agenda">
              Agenda
            </Link>
          </nav>
        </aside>

        <main className="p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}