import { Link } from 'react-router-dom'

export default function BookingLandingPage() {
  return (
    <div className="min-h-screen bg-white px-6 py-16">
      <div className="mx-auto max-w-5xl">
        <p className="mb-3 text-sm font-semibold text-violet-600">PsychoCitas</p>
        <h1 className="max-w-3xl text-5xl font-semibold tracking-tight">
          Reserva tu cita psicológica de forma simple
        </h1>
        <p className="mt-4 max-w-2xl text-slate-600">
          Consulta disponibilidad, agenda tu sesión y continúa con el pago.
        </p>

        <div className="mt-8 flex gap-3">
          <Link to="/login" className="rounded-2xl border border-slate-300 px-5 py-3">
            Staff
          </Link>
          <button className="rounded-2xl bg-slate-900 px-5 py-3 text-white">
            Reservar
          </button>
        </div>
      </div>
    </div>
  )
}