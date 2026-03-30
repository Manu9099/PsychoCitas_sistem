import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { CalendarDays, Clock3, UserRound } from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Button from '../../components/ui/Button'
import EmptyState from '../../components/ui/EmptyState'
import StatusBadge from '../../components/ui/StatusBadge'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '../../components/ui/Card'
import { citasApi } from '../../api/citasApi'

function normalizeItems(data) {
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.items)) return data.items
  if (Array.isArray(data?.data)) return data.data
  return []
}

function getCitaId(item, index) {
  return item?.id || item?.citaId || index
}

function getPacienteId(item) {
  return item?.pacienteId || item?.paciente?.id || null
}

function getPacienteNombre(item) {
  return (
    item?.pacienteNombre ||
    item?.paciente?.nombreCompleto ||
    item?.paciente?.nombre ||
    `${item?.paciente?.nombres || ''} ${item?.paciente?.apellidos || ''}`.trim() ||
    'Paciente'
  )
}

function getHora(item) {
  return (
    item?.horaInicio ||
    item?.fechaHora ||
    item?.fecha ||
    item?.inicio ||
    'Sin hora'
  )
}

function getEstado(item) {
  return item?.estado || item?.status || 'Pendiente'
}

export default function AgendaPage() {
  const navigate = useNavigate()

  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const loadAgenda = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await citasApi.getAgendaHoy()
      setItems(normalizeItems(data))
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo cargar la agenda'
      )
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadAgenda()
  }, [])

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Agenda de hoy"
        description="Revisa las citas programadas del día y entra al detalle del paciente."
        actions={
          <Button variant="secondary" onClick={loadAgenda} loading={loading}>
            Recargar
          </Button>
        }
      />

      {error ? (
        <Card>
          <CardContent>
            <p className="text-sm text-rose-600">{error}</p>
          </CardContent>
        </Card>
      ) : null}

      {!loading && items.length === 0 ? (
        <EmptyState
          title="No hay citas para hoy"
          description="Cuando existan citas programadas aparecerán aquí."
        />
      ) : null}

      {loading ? (
        <Card>
          <CardContent>
            <p className="text-sm text-slate-500">Cargando agenda...</p>
          </CardContent>
        </Card>
      ) : null}

      {items.length > 0 ? (
        <div className="grid gap-4">
          {items.map((item, index) => {
            const citaId = getCitaId(item, index)
            const pacienteId = getPacienteId(item)
            const pacienteNombre = getPacienteNombre(item)
            const hora = getHora(item)
            const estado = getEstado(item)

            return (
              <Card
                key={citaId}
                className="transition hover:-translate-y-0.5 hover:shadow-md"
              >
                <CardHeader>
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div className="flex items-center gap-3">
                      <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-slate-100">
                        <CalendarDays className="h-5 w-5 text-slate-700" />
                      </div>

                      <div>
                        <CardTitle>{pacienteNombre}</CardTitle>
                        <CardDescription>
                          Cita programada para hoy
                        </CardDescription>
                      </div>
                    </div>

                    <StatusBadge status={estado}>{estado}</StatusBadge>
                  </div>
                </CardHeader>

                <CardContent className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                  <div className="space-y-2 text-sm text-slate-500">
                    <div className="flex items-center gap-2">
                      <Clock3 className="h-4 w-4" />
                      <span>{hora}</span>
                    </div>

                    <div className="flex items-center gap-2">
                      <UserRound className="h-4 w-4" />
                      <span>{pacienteNombre}</span>
                    </div>
                  </div>

                  <div className="flex gap-2">
                    {pacienteId ? (
                      <Button
                        variant="secondary"
                        onClick={() => navigate(`/app/pacientes/${pacienteId}`)}
                      >
                        Ver paciente
                      </Button>
                    ) : null}
                  </div>
                </CardContent>
              </Card>
            )
          })}
        </div>
      ) : null}
    </div>
  )
}