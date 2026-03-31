import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  CalendarDays,
  Users,
  Wallet,
  ArrowRight,
  RefreshCcw,
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
import { dashboardApi } from '../../api/dashboardApi'
import { citasApi } from '../../api/citasApi'

// ============================================================================
// NORMALIZADORES
// ============================================================================

function normalizeAgendaItems(data) {
  if (!data) return []
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.items)) return data.items
  if (Array.isArray(data?.data)) return data.data
  return []
}

function getHora(item) {
  if (!item) return 'Sin hora'
  const hora =
    item?.horaInicio ||
    item?.fechaHora ||
    item?.fecha ||
    item?.inicio ||
    item?.hora ||
    null

  if (!hora) return 'Sin hora'

  try {
    const fecha = new Date(hora)
    return fecha.toLocaleTimeString('es-PE', {
      hour: '2-digit',
      minute: '2-digit',
    })
  } catch {
    return String(hora)
  }
}

function getPacienteNombre(item) {
  if (!item) return 'Paciente'

  return (
    item?.pacienteNombre ||
    item?.paciente?.nombreCompleto ||
    item?.paciente?.nombre ||
    `${item?.paciente?.nombres || ''} ${item?.paciente?.apellidos || ''}`.trim() ||
    `${item?.nombres || ''} ${item?.apellidos || ''}`.trim() ||
    item?.nombre ||
    'Paciente'
  )
}

function formatMoney(value) {
  return new Intl.NumberFormat('es-PE', {
    style: 'currency',
    currency: 'PEN',
    maximumFractionDigits: 2,
  }).format(Number(value || 0))
}

// ============================================================================
// COMPONENTE PRINCIPAL
// ============================================================================

export default function DashboardPage() {
  const navigate = useNavigate()

  // ─────────────────────────────────────────────────────────────────────────
  // ESTADOS
  // ─────────────────────────────────────────────────────────────────────────

  const [resumen, setResumen] = useState({
    citasHoy: 0,
    pacientesTotales: 0,
    totalPagado: 0,
  })
  const [agendaItems, setAgendaItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  // ─────────────────────────────────────────────────────────────────────────
  // EFECTOS
  // ─────────────────────────────────────────────────────────────────────────

  useEffect(() => {
    loadDashboard()
  }, [])

  // ─────────────────────────────────────────────────────────────────────────
  // API CALLS
  // ─────────────────────────────────────────────────────────────────────────

  const loadDashboard = async () => {
    setLoading(true)
    setError('')

    try {
      // Cargar resumen y agenda en paralelo
      const [resumenData, agendaData] = await Promise.all([
        dashboardApi.getResumen(),
        citasApi.getAgendaHoy(),
      ])

      // Normalizar resumen (soporta tanto camelCase como PascalCase)
      setResumen({
        citasHoy: resumenData?.citasHoy ?? resumenData?.CitasHoy ?? 0,
        pacientesTotales:
          resumenData?.pacientesTotales ?? resumenData?.PacientesTotales ?? 0,
        totalPagado: resumenData?.totalPagado ?? resumenData?.TotalPagado ?? 0,
      })

      // Normalizar agenda
      setAgendaItems(normalizeAgendaItems(agendaData))
    } catch (err) {
      setResumen({
        citasHoy: 0,
        pacientesTotales: 0,
        totalPagado: 0,
      })
      setAgendaItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo cargar el dashboard. Intenta nuevamente.'
      )
    } finally {
      setLoading(false)
    }
  }

  // ─────────────────────────────────────────────────────────────────────────
  // COMPUTED: TARJETAS DE MÉTRICAS
  // ─────────────────────────────────────────────────────────────────────────

  const metricCards = [
    {
      id: 'citas',
      title: 'Citas de hoy',
      value: loading ? '...' : String(resumen.citasHoy),
      description: 'Total programado para el día.',
      icon: CalendarDays,
      status: resumen.citasHoy > 0 ? 'activo' : 'pendiente',
      onClick: () => navigate('/app/agenda'),
    },
    {
      id: 'pacientes',
      title: 'Pacientes',
      value: loading ? '...' : String(resumen.pacientesTotales),
      description: 'Pacientes activos registrados.',
      icon: Users,
      status: resumen.pacientesTotales > 0 ? 'activo' : 'pendiente',
      onClick: () => navigate('/app/pacientes'),
    },
    {
      id: 'pagos',
      title: 'Total pagado',
      value: loading ? '...' : formatMoney(resumen.totalPagado),
      description: 'Suma pagada acumulada.',
      icon: Wallet,
      status: resumen.totalPagado > 0 ? 'success' : 'pendiente',
      onClick: () => navigate('/app/pagos'),
    },
  ]

  // ─────────────────────────────────────────────────────────────────────────
  // RENDER
  // ─────────────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Panel principal"
        description="Resumen operativo del sistema."
        actions={
          <Button variant="secondary" onClick={loadDashboard} loading={loading}>
            <RefreshCcw className="h-4 w-4" />
            Recargar
          </Button>
        }
      />

      {error && (
        <Card>
          <CardContent>
            <div className="flex items-start gap-3 rounded-2xl border border-rose-200 bg-rose-50 p-4 text-rose-700">
              <div className="mt-0.5 h-5 w-5 -flex-shrink-0 rounded-full bg-rose-200" />
              <p className="text-sm font-medium">{error}</p>
            </div>
          </CardContent>
        </Card>
      )}

      {/* ─────────────────────────────────────────────────────────────────────────
          TARJETAS DE MÉTRICAS
          ───────────────────────────────────────────────────────────────────────── */}

      <section className="grid gap-4 md:grid-cols-3">
        {metricCards.map(
          ({
            id,
            title,
            value,
            description,
            icon: Icon,
            status,
            onClick,
          }) => (
            <button key={id} onClick={onClick} className="text-left">
              <Card className="h-full overflow-hidden transition hover:-translate-y-0.5 hover:shadow-md">
                <CardContent className="p-5">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-slate-100">
                      <Icon className="h-5 w-5 text-slate-700" />
                    </div>

                    <StatusBadge status={status}>
                      {status === 'success'
                        ? 'Actualizado'
                        : status === 'activo'
                        ? 'Con datos'
                        : 'Pendiente'}
                    </StatusBadge>
                  </div>

                  <div className="mt-5">
                    <p className="text-sm text-slate-500">{title}</p>
                    <p className="mt-1 text-3xl font-semibold tracking-tight">
                      {value}
                    </p>
                    <p className="mt-2 text-sm leading-6 text-slate-500">
                      {description}
                    </p>
                  </div>
                </CardContent>
              </Card>
            </button>
          )
        )}
      </section>

      {/* ─────────────────────────────────────────────────────────────────────────
          AGENDA + ACCIONES RÁPIDAS
          ───────────────────────────────────────────────────────────────────────── */}

      <section className="grid gap-4 xl:grid-cols-[1.2fr_0.8fr]">
        <Card>
          <CardHeader>
            <CardTitle>Agenda del día</CardTitle>
            <CardDescription>
              Vista rápida de las próximas citas de hoy.
            </CardDescription>
          </CardHeader>

          <CardContent>
            {loading ? (
              <p className="text-sm text-slate-500">Cargando agenda...</p>
            ) : agendaItems.length === 0 ? (
              <EmptyState
                title="No hay citas para hoy"
                description="Cuando existan citas programadas aparecerán aquí."
              />
            ) : (
              <div className="space-y-3">
                {agendaItems.slice(0, 5).map((item, index) => (
                  <button
                    key={item?.id || item?.citaId || index}
                    onClick={() => navigate('/app/agenda')}
                    className="flex w-full items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
                  >
                    <div className="min-w-0">
                      <p className="truncate font-medium text-slate-900">
                        {getPacienteNombre(item)}
                      </p>
                      <p className="mt-1 text-sm text-slate-500">
                        {getHora(item)}
                      </p>
                    </div>

                    <div className="ml-4 flex items-center gap-3">
                      <StatusBadge status={item?.estado || 'Pendiente'}>
                        {item?.estado || 'Pendiente'}
                      </StatusBadge>
                      <ArrowRight className="h-4 w-4 -flex-shrink-0 text-slate-500" />
                    </div>
                  </button>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Acciones rápidas</CardTitle>
            <CardDescription>
              Accesos útiles para el flujo interno y externo.
            </CardDescription>
          </CardHeader>

          <CardContent className="grid gap-3">
            <button
              onClick={() => navigate('/app/agenda')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Ir a agenda</p>
                <p className="mt-1 text-sm text-slate-500">Ver citas del día</p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              onClick={() => navigate('/app/pacientes')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Pacientes</p>
                <p className="mt-1 text-sm text-slate-500">Búsqueda y detalle</p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              onClick={() => navigate('/app/pagos')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Pagos</p>
                <p className="mt-1 text-sm text-slate-500">Gestionar cobros</p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>

            <button
              onClick={() => navigate('/reservar')}
              className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4 text-left transition hover:bg-slate-100"
            >
              <div>
                <p className="font-medium text-slate-900">Reserva pública</p>
                <p className="mt-1 text-sm text-slate-500">Probar flujo externo</p>
              </div>
              <ArrowRight className="h-4 w-4 text-slate-500" />
            </button>
          </CardContent>
        </Card>
      </section>
    </div>
  )
}