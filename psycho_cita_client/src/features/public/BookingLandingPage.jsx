import { Link } from 'react-router-dom'

export default function BookingLandingPage() {
  return (
    <div className="min-h-screen bg-white text-slate-900">
      <section className="mx-auto flex min-h-screen max-w-6xl flex-col justify-center px-6 py-16">
        <p className="mb-3 text-sm font-medium text-violet-600">PsychoCitas</p>
        <h1 className="max-w-3xl text-4xl font-semibold tracking-tight md:text-6xl">
          Reserva tu cita psicológica de forma simple.
        </h1>
        <p className="mt-5 max-w-2xl text-base leading-7 text-slate-600">
          Consulta horarios disponibles, agenda tu sesión y continúa con el flujo de pago.
        </p>

        <div className="mt-8 flex flex-wrap gap-3">
          <Link
            to="/reservar"
            className="rounded-2xl bg-slate-900 px-5 py-3 text-sm font-medium text-white"
          >
            Reservar ahora
          </Link>

          <Link
            to="/login"
            className="rounded-2xl border border-slate-300 px-5 py-3 text-sm font-medium text-slate-700"
          >
            Acceso staff
          </Link>
        </div>
      </section>
    </div>
  )
}