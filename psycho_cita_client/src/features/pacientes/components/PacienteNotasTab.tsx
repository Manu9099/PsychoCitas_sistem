import { useEffect, useMemo, useState } from 'react'
import {
  AlertCircle,
  CalendarDays,
  CheckCircle2,
  FilePenLine,
  Save,
} from 'lucide-react'
import Button from '../../../components/ui/Button'
import EmptyState from '../../../components/ui/EmptyState'
import Input from '../../../components/ui/Input'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '../../../components/ui/Card'
import { citasApi } from '../../../api/citasApi'
import { notasApi } from '../../../api/notasApi'

// ============================================================================
// CONSTANTES Y MAPEOS
// ============================================================================

const TIPO_SESION_MAP = {
  0: 'Individual',
  1: 'Pareja',
  2: 'Familia',
  3: 'Grupo',
  4: 'Evaluación',
  5: 'Seguimiento',
}

const MODALIDAD_MAP = {
  0: 'Presencial',
  1: 'Videollamada',
  2: 'Telefónica',
}

const NIVEL_RIESGO_OPTIONS = ['', 'Bajo', 'Moderado', 'Alto', 'Crítico']

// ============================================================================
// TIPOS
// ============================================================================

interface Cita {
  id: string
  fechaInicio: string
  tipoSesion: number | string
  modalidad: number | string
  estado: string
}

interface Nota {
  resumenSesion?: string
  tecnicasUsadas?: string[]
  estadoAnimo?: number | null
  nivelAnsiedad?: number | null
  avanceObjetivos?: string
  tareasAsignadas?: string
  planProximaSesion?: string
  evaluacionRiesgo?: boolean
  nivelRiesgo?: string | null
  finalizada?: boolean
  actualizadoEn?: string | null
}

interface FormState extends Nota {
  tecnicasUsadasText: string
}

// ============================================================================
// NORMALIZADORES Y UTILIDADES
// ============================================================================

function formatDateTime(value: string | null | undefined): string {
  if (!value) return 'Sin fecha'

  try {
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return value

    return date.toLocaleString('es-PE', {
      year: 'numeric',
      month: 'short',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    })
  } catch {
    return 'Fecha inválida'
  }
}

function getTipoSesionLabel(tipoSesion: number | string | undefined): string {
  if (tipoSesion === undefined || tipoSesion === null) return 'No especificado'

  const tipo = typeof tipoSesion === 'number' ? tipoSesion : parseInt(tipoSesion, 10)
  return TIPO_SESION_MAP[tipo as keyof typeof TIPO_SESION_MAP] || String(tipoSesion)
}

function getModalidadLabel(modalidad: number | string | undefined): string {
  if (modalidad === undefined || modalidad === null) return 'No especificada'

  const mod = typeof modalidad === 'number' ? modalidad : parseInt(modalidad, 10)
  return MODALIDAD_MAP[mod as keyof typeof MODALIDAD_MAP] || String(modalidad)
}

function createEmptyForm(): FormState {
  return {
    resumenSesion: '',
    tecnicasUsadasText: '',
    estadoAnimo: null,
    nivelAnsiedad: null,
    avanceObjetivos: '',
    tareasAsignadas: '',
    planProximaSesion: '',
    evaluacionRiesgo: false,
    nivelRiesgo: '',
    finalizada: false,
    actualizadoEn: null,
  }
}

function mapNoteToForm(nota: Nota | null | undefined): FormState {
  if (!nota) return createEmptyForm()

  return {
    resumenSesion: nota.resumenSesion || '',
    tecnicasUsadasText: Array.isArray(nota.tecnicasUsadas)
      ? nota.tecnicasUsadas.join(', ')
      : '',
    estadoAnimo: nota.estadoAnimo ?? null,
    nivelAnsiedad: nota.nivelAnsiedad ?? null,
    avanceObjetivos: nota.avanceObjetivos || '',
    tareasAsignadas: nota.tareasAsignadas || '',
    planProximaSesion: nota.planProximaSesion || '',
    evaluacionRiesgo: Boolean(nota.evaluacionRiesgo),
    nivelRiesgo: nota.nivelRiesgo || '',
    finalizada: Boolean(nota.finalizada),
    actualizadoEn: nota.actualizadoEn || null,
  }
}

function buildPayload(form: FormState): Partial<Nota> {
  return {
    resumenSesion: form.resumenSesion,
    tecnicasUsadas: form.tecnicasUsadasText
      .split(',')
      .map((item) => item.trim())
      .filter(Boolean),
    estadoAnimo: form.estadoAnimo === null ? null : form.estadoAnimo,
    nivelAnsiedad: form.nivelAnsiedad === null ? null : form.nivelAnsiedad,
    avanceObjetivos: form.avanceObjetivos,
    tareasAsignadas: form.tareasAsignadas,
    planProximaSesion: form.planProximaSesion,
    evaluacionRiesgo: form.evaluacionRiesgo,
    nivelRiesgo: form.evaluacionRiesgo ? form.nivelRiesgo || null : null,
  }
}

// ============================================================================
// COMPONENTE: SELECTOR DE SESIONES
// ============================================================================

interface SessionSelectorProps {
  citas: Cita[]
  selectedCitaId: string
  onSelect: (id: string) => void
}

function SessionSelector({ citas, selectedCitaId, onSelect }: SessionSelectorProps) {
  return (
    <div className="grid gap-3">
      {citas.map((cita) => {
        const isActive = cita.id === selectedCitaId
        const tipoLabel = getTipoSesionLabel(cita.tipoSesion)
        const modalidadLabel = getModalidadLabel(cita.modalidad)

        return (
          <button
            key={cita.id}
            type="button"
            onClick={() => onSelect(cita.id)}
            className={[
              'rounded-2xl border p-4 text-left transition',
              isActive
                ? 'border-slate-900 bg-slate-900 text-white'
                : 'border-slate-200 bg-white hover:bg-slate-50',
            ].join(' ')}
          >
            <div className="flex items-start justify-between gap-3">
              <div>
                <p className="font-medium">{formatDateTime(cita.fechaInicio)}</p>
                <p
                  className={[
                    'mt-1 text-sm',
                    isActive ? 'text-slate-200' : 'text-slate-500',
                  ].join(' ')}
                >
                  {tipoLabel} · {modalidadLabel}
                </p>
              </div>
              <span
                className={[
                  'rounded-full px-2.5 py-1 text-xs font-medium',
                  isActive
                    ? 'bg-white/10 text-white'
                    : 'bg-slate-100 text-slate-700',
                ].join(' ')}
              >
                {cita.estado}
              </span>
            </div>
          </button>
        )
      })}
    </div>
  )
}

// ============================================================================
// COMPONENTE PRINCIPAL
// ============================================================================

interface PacienteNotasTabProps {
  pacienteId: string
}

export default function PacienteNotasTab({ pacienteId }: PacienteNotasTabProps) {
  // ─────────────────────────────────────────────────────────────────────────
  // ESTADOS
  // ─────────────────────────────────────────────────────────────────────────

  const [citas, setCitas] = useState<Cita[]>([])
  const [selectedCitaId, setSelectedCitaId] = useState('')
  const [loadingCitas, setLoadingCitas] = useState(true)
  const [loadingNota, setLoadingNota] = useState(false)
  const [saving, setSaving] = useState(false)
  const [finalizing, setFinalizing] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [form, setForm] = useState<FormState>(createEmptyForm())

  // ─────────────────────────────────────────────────────────────────────────
  // EFECTOS: Cargar citas del paciente
  // ─────────────────────────────────────────────────────────────────────────

  useEffect(() => {
    let ignore = false

    const loadCitas = async () => {
      setLoadingCitas(true)
      setError('')

      try {
        const data = await citasApi.getByPaciente(pacienteId)
        const normalized = Array.isArray(data) ? data : []
        const ordered = normalized.sort(
          (a, b) =>
            new Date(b?.fechaInicio || 0).getTime() -
            new Date(a?.fechaInicio || 0).getTime()
        )

        if (!ignore) {
          setCitas(ordered)
          setSelectedCitaId((current) => current || ordered[0]?.id || '')
        }
      } catch (err: any) {
        if (!ignore) {
          setError(
            err?.response?.data?.message ||
              err?.response?.data?.title ||
              'No se pudieron cargar las sesiones del paciente.'
          )
        }
      } finally {
        if (!ignore) {
          setLoadingCitas(false)
        }
      }
    }

    loadCitas()

    return () => {
      ignore = true
    }
  }, [pacienteId])

  // ─────────────────────────────────────────────────────────────────────────
  // EFECTOS: Cargar nota de sesión
  // ─────────────────────────────────────────────────────────────────────────

  useEffect(() => {
    let ignore = false

    const loadNota = async () => {
      if (!selectedCitaId) {
        setForm(createEmptyForm())
        return
      }

      setLoadingNota(true)
      setError('')
      setSuccess('')

      try {
        const data = await notasApi.getByCita(selectedCitaId)
        if (!ignore) {
          setForm(mapNoteToForm(data))
        }
      } catch (err: any) {
        if (!ignore) {
          const status = err?.response?.status
          if (status === 404) {
            setForm(createEmptyForm())
          } else {
            setError(
              err?.response?.data?.message ||
                err?.response?.data?.title ||
                'No se pudo cargar la nota de la sesión.'
            )
          }
        }
      } finally {
        if (!ignore) {
          setLoadingNota(false)
        }
      }
    }

    loadNota()

    return () => {
      ignore = true
    }
  }, [selectedCitaId])

  // ─────────────────────────────────────────────────────────────────────────
  // COMPUTED
  // ─────────────────────────────────────────────────────────────────────────

  const selectedCita = useMemo(
    () => citas.find((cita) => cita.id === selectedCitaId) || null,
    [citas, selectedCitaId]
  )

  // ─────────────────────────────────────────────────────────────────────────
  // HANDLERS
  // ─────────────────────────────────────────────────────────────────────────

  const handleFieldChange = (field: keyof FormState) => (event: any) => {
    const value =
      event?.target?.type === 'checkbox' ? event.target.checked : event?.target?.value

    setForm((prev) => ({
      ...prev,
      [field]: value,
      ...(field === 'evaluacionRiesgo' && !value ? { nivelRiesgo: '' } : {}),
    }))
    setError('')
    setSuccess('')
  }

  const handleSave = async () => {
    if (!selectedCitaId) return

    setSaving(true)
    setError('')
    setSuccess('')

    try {
      const saved = await notasApi.guardar(selectedCitaId, buildPayload(form))
      setForm(mapNoteToForm(saved))
      setSuccess('Nota guardada correctamente.')
    } catch (err: any) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo guardar la nota.'
      )
    } finally {
      setSaving(false)
    }
  }

  const handleFinalize = async () => {
    if (!selectedCitaId) return

    setFinalizing(true)
    setError('')
    setSuccess('')

    try {
      const result = await notasApi.finalizar(selectedCitaId)
      setForm(mapNoteToForm(result))
      setSuccess('Nota finalizada correctamente.')
    } catch (err: any) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo finalizar la nota.'
      )
    } finally {
      setFinalizing(false)
    }
  }

  // ─────────────────────────────────────────────────────────────────────────
  // RENDER
  // ─────────────────────────────────────────────────────────────────────────

  if (loadingCitas) {
    return (
      <Card>
        <CardContent>
          <p className="text-sm text-slate-500">Cargando sesiones del paciente...</p>
        </CardContent>
      </Card>
    )
  }

  if (citas.length === 0) {
    return (
      <EmptyState
        title="Sin sesiones registradas"
        description="Las notas se gestionan por sesión. Cuando el paciente tenga citas creadas, podrás documentarlas desde aquí."
      />
    )
  }

  return (
    <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
      {/* ═══════════════════════════════════════════════════════════════════════
          COLUMNA IZQUIERDA: SELECTOR DE SESIONES
          ═══════════════════════════════════════════════════════════════════════ */}

      <Card>
        <CardHeader>
          <CardTitle>Sesiones del paciente</CardTitle>
          <CardDescription>Selecciona la cita que quieres documentar.</CardDescription>
        </CardHeader>
        <CardContent>
          <SessionSelector
            citas={citas}
            selectedCitaId={selectedCitaId}
            onSelect={setSelectedCitaId}
          />
        </CardContent>
      </Card>

      {/* ═══════════════════════════════════════════════════════════════════════
          COLUMNA DERECHA: NOTA DE SESIÓN
          ═══════════════════════════════════════════════════════════════════════ */}

      <Card>
        <CardHeader>
          <CardTitle>Nota de sesión</CardTitle>
          <CardDescription>
            {selectedCita
              ? `Trabajando sobre la sesión del ${formatDateTime(selectedCita.fechaInicio)}.`
              : 'Selecciona una sesión.'}
          </CardDescription>
        </CardHeader>

        <CardContent className="space-y-5">
          {/* Mensajes de error/éxito */}
          {(error || success) && (
            <div
              className={[
                'rounded-2xl border px-4 py-3 text-sm',
                error
                  ? 'border-rose-200 bg-rose-50 text-rose-700'
                  : 'border-emerald-200 bg-emerald-50 text-emerald-700',
              ].join(' ')}
            >
              <div className="flex items-start gap-2">
                {error ? (
                  <AlertCircle className="mt-0.5 h-4 w-4 shrink-0" />
                ) : (
                  <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0" />
                )}
                <span>{error || success}</span>
              </div>
            </div>
          )}

          {loadingNota ? (
            <p className="text-sm text-slate-500">Cargando nota...</p>
          ) : (
            <>
              {/* Evaluaciones rápidas */}
              <div className="grid gap-4 md:grid-cols-2">
                <Input
                  label="Estado de ánimo (1-10)"
                  type="number"
                  min="1"
                  max="10"
                  value={form.estadoAnimo ?? ''}
                  onChange={handleFieldChange('estadoAnimo')}
                />
                <Input
                  label="Nivel de ansiedad (1-10)"
                  type="number"
                  min="1"
                  max="10"
                  value={form.nivelAnsiedad ?? ''}
                  onChange={handleFieldChange('nivelAnsiedad')}
                />
              </div>

              {/* Resumen de sesión */}
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Resumen de sesión
                </label>
                <textarea
                  value={form.resumenSesion}
                  onChange={handleFieldChange('resumenSesion')}
                  rows={5}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  placeholder="Describe el contenido principal de la sesión."
                />
              </div>

              {/* Técnicas usadas */}
              <Input
                label="Técnicas usadas"
                hint="Separar por comas. Ej.: reestructuración cognitiva, psicoeducación"
                value={form.tecnicasUsadasText}
                onChange={handleFieldChange('tecnicasUsadasText')}
              />

              {/* Avance de objetivos */}
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Avance de objetivos
                </label>
                <textarea
                  value={form.avanceObjetivos}
                  onChange={handleFieldChange('avanceObjetivos')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  placeholder="Resume avances observados respecto al plan terapéutico."
                />
              </div>

              {/* Tareas asignadas */}
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Tareas asignadas
                </label>
                <textarea
                  value={form.tareasAsignadas}
                  onChange={handleFieldChange('tareasAsignadas')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  placeholder="Tareas o compromisos acordados para el paciente."
                />
              </div>

              {/* Plan para próxima sesión */}
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Plan para próxima sesión
                </label>
                <textarea
                  value={form.planProximaSesion}
                  onChange={handleFieldChange('planProximaSesion')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  placeholder="Define el enfoque o temas de seguimiento."
                />
              </div>

              {/* Evaluación de riesgo */}
              <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                <div className="flex items-start gap-3">
                  <input
                    id="evaluacion-riesgo"
                    type="checkbox"
                    checked={form.evaluacionRiesgo}
                    onChange={handleFieldChange('evaluacionRiesgo')}
                    className="mt-1 h-4 w-4 rounded border-slate-300 text-violet-600 focus:ring-violet-500"
                  />
                  <div className="min-w-0">
                    <label
                      htmlFor="evaluacion-riesgo"
                      className="block text-sm font-medium text-slate-900"
                    >
                      Evaluación de riesgo
                    </label>
                    <p className="mt-1 text-sm text-slate-500">
                      Actívalo solo si esta sesión amerita registrar un nivel de riesgo.
                    </p>
                  </div>
                </div>

                {form.evaluacionRiesgo && (
                  <div className="mt-4">
                    <label className="mb-2 block text-sm font-medium text-slate-700">
                      Nivel de riesgo
                    </label>
                    <select
                      value={form.nivelRiesgo || ''}
                      onChange={handleFieldChange('nivelRiesgo')}
                      className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                    >
                      {NIVEL_RIESGO_OPTIONS.map((option) => (
                        <option key={option || 'none'} value={option}>
                          {option || 'Seleccionar nivel'}
                        </option>
                      ))}
                    </select>
                  </div>
                )}
              </div>

              {/* Botones de acción */}
              <div className="flex flex-wrap items-center gap-3 border-t border-slate-100 pt-4">
                <Button onClick={handleSave} loading={saving}>
                  <Save className="h-4 w-4" />
                  Guardar nota
                </Button>

                <Button
                  variant="secondary"
                  onClick={handleFinalize}
                  loading={finalizing}
                  disabled={form.finalizada}
                >
                  <FilePenLine className="h-4 w-4" />
                  {form.finalizada ? 'Nota finalizada' : 'Finalizar nota'}
                </Button>

                <div className="ml-auto flex items-center gap-2 text-sm text-slate-500">
                  <CalendarDays className="h-4 w-4" />
                  <span>Actualizado: {formatDateTime(form.actualizadoEn)}</span>
                </div>
              </div>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  )
}