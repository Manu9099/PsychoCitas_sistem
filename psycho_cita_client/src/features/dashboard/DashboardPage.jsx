export default function DashboardPage() {
  return (
    <div className="space-y-4">
      <h1 className="text-3xl font-semibold">Panel</h1>
      <div className="grid gap-4 md:grid-cols-3">
        <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm">
          <p className="text-sm text-slate-500">Agenda</p>
          <p className="mt-1 text-xl font-semibold">Hoy</p>
        </div>
        <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm">
          <p className="text-sm text-slate-500">Pacientes</p>
          <p className="mt-1 text-xl font-semibold">Buscar</p>
        </div>
        <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm">
          <p className="text-sm text-slate-500">Pagos</p>
          <p className="mt-1 text-xl font-semibold">Checkout</p>
        </div>
      </div>
    </div>
  )
}