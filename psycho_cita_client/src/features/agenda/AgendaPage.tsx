import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { CalendarDays, Clock3, UserRound, AlertCircle, Filter, X } from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
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

interface Cita {
  id: string
  citaId?: string
  pacienteNombre?: string
  paciente_nombre?: string
  estado?: string
  status?: string
  fechaInicio?: string
  horaInicio?: string
  fechaHora?: string
  inicio?: string
  fechaFin?: string
  fin?: string
  tipo_sesion?: string
  tipoSesion?: string
  modalidad?: string
}

interface FilterState {
  fechaInicio: string
  fechaFin: string
  estado: string
  modalidad: string
}

const ESTADO_OPTIONS = [
  { value: '', label: 'Todos los estados' },
  { value: 'Programada', label: 'Programada' },
  { value: 'Confirmada', label: 'Confirmada' },
  { value: 'Completada', label: 'Completada' },
  { value: 'Cancelada', label: 'Cancelada' },
  { value: 'NoAsistio', label: 'No asistió' },
  { value: 'Reagendada', label: 'Reagendada' },
]

const MODALIDAD_OPTIONS = [
  { value: '', label: 'Todas las modalidades' },
  { value: 'Presencial', label: 'Presencial' },
  { value: 'Videollamada', label: 'Videollamada' },
  { value: 'Telefonica', label: 'Telefónica' },
]

function normalizeItems(data: unknown): Cita[] {
  if (Array.isArray(data)) return data
  if (data && typeof data === 'object') {
    if (Array.isArray((data as any).items)) return (data as any).items
    if (Array.isArray((data as any).data)) return (data as any).data
  }
  return []
}

function getEstado(cita: Cita): string {
  return cita.estado || cita.status || 'Pendiente'
}

function getPacienteNombre(cita: Cita): string {
  return cita.pacienteNombre || cita.paciente_nombre || 'Paciente'
}

function getFechaRaw(cita: Cita): string {
  return cita.fechaInicio || cita.horaInicio || cita.fechaHora || cita.inicio || ''
}

function getHora(cita: Cita): string {
  const inicio = getFechaRaw(cita)
  const fin = cita.fechaFin || cita.fin

  if (!inicio) return 'Sin hora'

  try {
    const inicioDate = new Date(inicio)
    const inicioStr = inicioDate.toLocaleTimeString('es-PE', {
      hour: '2-digit',
      minute: '2-digit',
    })

    if (!fin) return inicioStr

    const finDate = new Date(fin)
    const finStr = finDate.toLocaleTimeString('es-PE', {
      hour: '2-digit',
      minute: '2-digit',
    })

    return `${inicioStr} - ${finStr}`
  } catch {
    return 'Hora inválida'
  }
}

function getFecha(cita: Cita): string {
  const fecha = getFechaRaw(cita)
  if (!fecha) return 'Sin fecha'

  try {
    return new Date(fecha).toLocaleDateString('es-PE')
  } catch {
    return 'Fecha inválida'
  }
}

function getModalidad(cita: Cita): string {
  return cita.modalidad || 'No especificada'
}

function filterCitas(citas: Cita[], filters: FilterState): Cita[] {
  return citas.filter((cita) => {
    const rawFecha = getFechaRaw(cita)
    if (!rawFecha) return false

    const fechaCita = new Date(rawFecha)

    if (filters.fechaInicio) {
      const desde = new Date(filters.fechaInicio)
      desde.setHours(0, 0, 0, 0)
      if (fechaCita < desde) return false
    }

    if (filters.fechaFin) {
      const hasta = new Date(filters.fechaFin)
      hasta.setHours(23, 59, 59, 999)
      if (fechaCita > hasta) return false
    }

    if (filters.estado && getEstado(cita) !== filters.estado) {
      return false
    }

    if (filters.modalidad && getModalidad(cita) !== filters.modalidad) {
      return false
    }

    return true
  })
}

export default function AgendaPage() {
  const navigate = useNavigate()

  const [citas, setCitas] = useState<Cita[]>([])
  const [loading, setLoading] = useState(true)
  const [actionLoading, setActionLoading] = useState<Record<string, boolean>>({})
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [showFilters, setShowFilters] = useState(false)

  const [filters, setFilters] = useState<FilterState>({
    fechaInicio: '',
    fechaFin: '',
    estado: '',
    modalidad: '',
  })

  const citasFiltradas = useMemo(() => {
    return filterCitas(citas, filters)
  }, [citas, filters])

  const hasActiveFilters = useMemo(() => {
    return !!(filters.fechaInicio || filters.fechaFin || filters.estado || filters.modalidad)
  }, [filters])

  const loadAgenda = async () => {
    setLoading(true)
    setError('')
    setSuccessMessage('')

    try {
      const data = await citasApi.getAll()
      const normalized = normalizeItems(data)
      setCitas(normalized)
      setSuccessMessage(`Se cargaron ${normalized.length} cita(s)`)
    } catch (err: any) {
      setCitas([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo cargar la agenda'
      )
    } finally {
      setLoading(false)
    }
  }

  const handleFilterChange = (key: keyof FilterState, value: string) => {
    setFilters((prev) => ({ ...prev, [key]: value }))
  }

  const handleClearFilters = () => {
    setFilters({
      fechaInicio: '',
      fechaFin: '',
      estado: '',
      modalidad: '',
    })
  }

  const runAction = async (key: string, fn: () => Promise<void>) => {
    setActionLoading((prev) => ({ ...prev, [key]: true }))
    setError('')
    setSuccessMessage('')

    try {
      await fn()
      setSuccessMessage('Acción completada exitosamente')
      await loadAgenda()
    } catch (err: any) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo completar la acción'
      )
    } finally {
      setActionLoading((prev) => ({ ...prev, [key]: false }))
    }
  }

  const handleCompletar = async (id: string) => {
    await runAction(`completar-${id}`, () => citasApi.completar(id))
  }

  const handleNoAsistio = async (id: string) => {
    await runAction(`noasistio-${id}`, () => citasApi.noAsistio(id))
  }

  const handleCancelar = async (id: string) => {
    const motivo = window.prompt('Motivo de cancelación:')
    if (!motivo) return
    await runAction(`cancelar-${id}`, () => citasApi.cancelar(id, motivo))
  }

  useEffect(() => {
    loadAgenda()
  }, [])

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Agenda"
        description="Listado de todas las citas con filtros por rango de fechas."
        actions={
          <Button
            variant="secondary"
            onClick={() => setShowFilters(!showFilters)}
            className="gap-2"
          >
            <Filter className="h-4 w-4" />
            Filtros
          </Button>
        }
      />

      {error && (
        <Card className="border-rose-200 bg-rose-50">
          <CardContent>
            <div className="flex items-start gap-3 text-rose-700">
              <AlertCircle className="mt-0.5 h-5 w-5 -flex-shrink-0" />
              <p className="text-sm font-medium">{error}</p>
            </div>
          </CardContent>
        </Card>
      )}

      {successMessage && (
        <Card className="border-emerald-200 bg-emerald-50">
          <CardContent>
            <div className="flex items-start gap-3 text-emerald-700">
              <div className="mt-0.5 h-5 w-5 -flex-shrink-0 rounded-full bg-emerald-300" />
              <p className="text-sm font-medium">{successMessage}</p>
            </div>
          </CardContent>
        </Card>
      )}

      {showFilters && (
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardTitle>Filtros avanzados</CardTitle>
              {hasActiveFilters && (
                <Button
                  variant="secondary"
                  size="sm"
                  onClick={handleClearFilters}
                  className="gap-2"
                >
                  <X className="h-4 w-4" />
                  Limpiar filtros
                </Button>
              )}
            </div>
          </CardHeader>

          <CardContent>
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              <Input
                label="Fecha desde"
                type="date"
                value={filters.fechaInicio}
                onChange={(e) => handleFilterChange('fechaInicio', e.target.value)}
              />

              <Input
                label="Fecha hasta"
                type="date"
                value={filters.fechaFin}
                onChange={(e) => handleFilterChange('fechaFin', e.target.value)}
              />

              <div>
                <label className="block text-sm font-medium text-slate-700">Estado</label>
                <select
                  value={filters.estado}
                  onChange={(e) => handleFilterChange('estado', e.target.value)}
                  className="mt-2 w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
                >
                  {ESTADO_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700">Modalidad</label>
                <select
                  value={filters.modalidad}
                  onChange={(e) => handleFilterChange('modalidad', e.target.value)}
                  className="mt-2 w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
                >
                  {MODALIDAD_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      <div className="flex justify-end">
        <Button onClick={loadAgenda} loading={loading} variant="secondary">
          Recargar
        </Button>
      </div>

      {loading ? (
        <Card>
          <CardContent>
            <p className="text-sm text-slate-500">Cargando agenda...</p>
          </CardContent>
        </Card>
      ) : citasFiltradas.length === 0 ? (
        <EmptyState
          title={hasActiveFilters ? 'No hay citas que coincidan' : 'No hay citas registradas'}
          description={
            hasActiveFilters
              ? 'Intenta cambiar los filtros.'
              : 'Cuando existan citas aparecerán aquí.'
          }
        />
      ) : (
        <div className="space-y-4">
          <p className="text-sm font-medium text-slate-600">
            Se muestran {citasFiltradas.length} de {citas.length} cita(s)
          </p>

          <div className="grid gap-4">
            {citasFiltradas.map((cita) => {
              const citaId = cita.id || cita.citaId || ''
              const estado = getEstado(cita)

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
                          <CardTitle className="text-lg">
                            {getPacienteNombre(cita)}
                          </CardTitle>
                          <CardDescription>{getFecha(cita)}</CardDescription>
                        </div>
                      </div>

                      <StatusBadge status={estado}>{estado}</StatusBadge>
                    </div>
                  </CardHeader>

                  <CardContent className="space-y-4">
                    <div className="grid gap-3 text-sm text-slate-600 md:grid-cols-3">
                      <div className="flex items-center gap-2">
                        <Clock3 className="h-4 w-4 text-slate-400" />
                        <span>{getHora(cita)}</span>
                      </div>

                      <div className="flex items-center gap-2">
                        <UserRound className="h-4 w-4 text-slate-400" />
                        <span className="capitalize">{getModalidad(cita)}</span>
                      </div>

                      <div className="text-slate-500">
                        Tipo: {cita.tipoSesion || cita.tipo_sesion || 'No especificado'}
                      </div>
                    </div>

                    <div className="flex flex-wrap gap-2 pt-2">
                      <Button
                        variant="secondary"
                        size="sm"
                        onClick={() => navigate(`/app/citas/${citaId}`)}
                      >
                        Ver detalle
                      </Button>

                      {(estado === 'Programada' || estado === 'Confirmada') && (
                        <>
                          <Button
                            size="sm"
                            onClick={() => handleCompletar(citaId)}
                            loading={!!actionLoading[`completar-${citaId}`]}
                          >
                            Completar
                          </Button>

                          <Button
                            variant="secondary"
                            size="sm"
                            onClick={() => handleNoAsistio(citaId)}
                            loading={!!actionLoading[`noasistio-${citaId}`]}
                          >
                            No asistió
                          </Button>

                          <Button
                            variant="danger"
                            size="sm"
                            onClick={() => handleCancelar(citaId)}
                            loading={!!actionLoading[`cancelar-${citaId}`]}
                          >
                            Cancelar
                          </Button>
                        </>
                      )}
                    </div>
                  </CardContent>
                </Card>
              )
            })}
          </div>
        </div>
      )}
    </div>
  )
}