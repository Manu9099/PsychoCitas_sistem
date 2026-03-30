import { useNavigate } from 'react-router-dom'
import {
  CalendarDays,
  Users,
  CreditCard,
  ArrowRight,
} from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Button from '../../components/ui/Button'
import StatusBadge from '../../components/ui/StatusBadge'
import EmptyState from '../../components/ui/EmptyState'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '../../components/ui/Card'

const quickStats = [
  {
    title: 'Agenda',
    value: 'Hoy',
    description: 'Consulta las citas programadas del día.',
    icon: CalendarDays,
    status: 'pendiente',
  },
  {
    title: 'Pacientes',
    value: 'Buscar',
    description: 'Revisa fichas y datos principales.',
    icon: Users,
    status: 'activo',
  },
  {
    title: 'Pagos',
    value: 'Checkout',
    description: 'Gestiona cobros y estado de pago.',
    icon: CreditCard,
    status: 'warning',
  },
]

export default function DashboardPage() {
  const navigate = useNavigate()

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Panel principal"
        description="Acceso rápido a la agenda, pacientes y flujo operativo del sistema."
        actions={
          <Button onClick={() => navigate('/app/agenda')}>
            Ver agenda
          </Button>
        }
      />

      <section className="grid gap-4 md:grid-cols-3">
        {quickStats.map(
          ({ title, value, description, icon: Icon, status }) => (
            <Card key={title} className="overflow-hidden">
              <CardContent className="p-5">
                <div className="flex items-start justify-between gap-4">
                  <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-slate-100">
                    <Icon className="h-5 w-5 text-slate-700" />
                  </div>

                  <StatusBadge status={status}>
                    {status === 'warning'
                      ? 'Atención'
                      : status === 'activo'
                      ? 'Disponible'
                      : 'Pendiente'}
                  </StatusBadge>
                </div>

                <div className="mt-5">
                  <p className="text-sm text-slate-500">{title}</p>
                  <p className="mt-1 text-2xl font-semibold tracking-tight">
                    {value}
                  </p>
                  <p className="mt-2 text-sm leading-6 text-slate-500">
                    {description}
                  </p>
                </div>
              </CardContent>
            </Card>
          )
        )}
      </section>

      <section className="grid gap-4 xl:grid-cols-[1.3fr_1fr]">
        <Card>
          <CardHeader>
            <CardTitle>Acciones rápidas</CardTitle>
            <CardDescription>
              Lo más útil para seguir construyendo el flujo interno.
            </CardDescription>
          </CardHeader>

          <CardContent className="grid gap-3 md:grid-cols-2">
            <button
              onClick={() => navigate('/app/agenda')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Ir a agenda</p>
                <p className="mt-1 text-sm text-slate-500">
                  Ver citas del día
                </p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              onClick={() => navigate('/app/pacientes')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Pacientes</p>
                <p className="mt-1 text-sm text-slate-500">
                  Búsqueda y detalle
                </p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              onClick={() => navigate('/reservar')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Reserva pública</p>
                <p className="mt-1 text-sm text-slate-500">
                  Probar flujo externo
                </p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Pagos</p>
                <p className="mt-1 text-sm text-slate-500">
                  Próximo módulo
                </p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>
          </CardContent>
        </Card>

        <EmptyState
          title="Aún no hay métricas conectadas"
          description="Cuando enlacemos la API podremos mostrar citas del día, pagos pendientes y pacientes recientes."
        />
      </section>
    </div>
  )
}