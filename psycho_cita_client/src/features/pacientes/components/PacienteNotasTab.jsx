import { useEffect, useMemo, useState } from 'react'
import { AlertCircle, CalendarDays, CheckCircle2, FilePenLine, Save } from 'lucide-react'
import Button from '../../../components/ui/Button'
import EmptyState from '../../../components/ui/EmptyState'
import Input from '../../../components/ui/Input'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../../components/ui/Card'
import { citasApi } from '../../../api/citasApi'
import { notasApi } from '../../../api/notasApi'

const RISK_OPTIONS = ['', 'Bajo', 'Moderado', 'Alto', 'Critico']

function createEmptyForm() {
  return {
    resumenSesion: '',
    tecnicasUsadasText: '',
    estadoAnimo: '',
    nivelAnsiedad: '',
    avanceObjetivos: '',
    tareasAsignadas: '',
    planProximaSesion: '',
    evaluacionRiesgo: false,
    nivelRiesgo: '',
    finalizada: false,
    actualizadoEn: null,
  }
}

function formatDateTime(value) {
  if (!value) return 'Sin fecha'

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleString('es-PE', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function mapNoteToForm(note) {
  return {
    resumenSesion: note?.resumenSesion || '',
    tecnicasUsadasText: Array.isArray(note?.tecnicasUsadas)
      ? note.tecnicasUsadas.join(', ')
      : '',
    estadoAnimo: note?.estadoAnimo ?? '',
    nivelAnsiedad: note?.nivelAnsiedad ?? '',
    avanceObjetivos: note?.avanceObjetivos || '',
    tareasAsignadas: note?.tareasAsignadas || '',
    planProximaSesion: note?.planProximaSesion || '',
    evaluacionRiesgo: Boolean(note?.evaluacionRiesgo),
    nivelRiesgo: note?.nivelRiesgo || '',
    finalizada: Boolean(note?.finalizada),
    actualizadoEn: note?.actualizadoEn || null,
  }
}

function buildPayload(form) {
  return {
    resumenSesion: form.resumenSesion,
    tecnicasUsadas: form.tecnicasUsadasText
      .split(',')
      .map((item) => item.trim())
      .filter(Boolean),
    estadoAnimo: form.estadoAnimo === '' ? null : Number(form.estadoAnimo),
    nivelAnsiedad: form.nivelAnsiedad === '' ? null : Number(form.nivelAnsiedad),
    avanceObjetivos: form.avanceObjetivos,
    tareasAsignadas: form.tareasAsignadas,
    planProximaSesion: form.planProximaSesion,
    evaluacionRiesgo: form.evaluacionRiesgo,
    nivelRiesgo: form.evaluacionRiesgo ? form.nivelRiesgo || null : null,
  }
}

function SessionSelector({ citas, selectedCitaId, onSelect }) {
  return (
    <div className="grid gap-3">
      {citas.map((cita) => {
        const isActive = cita.id === selectedCitaId

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
                <p className={['mt-1 text-sm', isActive ? 'text-slate-200' : 'text-slate-500'].join(' ')}>
                  {cita.tipoSesion} · {cita.modalidad}
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

export default function PacienteNotasTab({ pacienteId }) {
  const [citas, setCitas] = useState([])
  const [selectedCitaId, setSelectedCitaId] = useState('')
  const [loadingCitas, setLoadingCitas] = useState(true)
  const [loadingNota, setLoadingNota] = useState(false)
  const [saving, setSaving] = useState(false)
  const [finalizing, setFinalizing] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [form, setForm] = useState(createEmptyForm())

  useEffect(() => {
    let ignore = false

    const loadCitas = async () => {
      setLoadingCitas(true)
      setError('')

      try {
        const data = await citasApi.getByPaciente(pacienteId)
        const normalized = Array.isArray(data) ? data : []
        const ordered = normalized.sort(
          (a, b) => new Date(b?.fechaInicio || 0) - new Date(a?.fechaInicio || 0),
        )

        if (!ignore) {
          setCitas(ordered)
          setSelectedCitaId((current) => current || ordered[0]?.id || '')
        }
      } catch (err) {
        if (!ignore) {
          setError(
            err?.response?.data?.message ||
              err?.response?.data?.title ||
              'No se pudieron cargar las sesiones del paciente.',
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
      } catch (err) {
        if (!ignore) {
          const status = err?.response?.status
          if (status === 404) {
            setForm(createEmptyForm())
          } else {
            setError(
              err?.response?.data?.message ||
                err?.response?.data?.title ||
                'No se pudo cargar la nota de la sesión.',
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

  const selectedCita = useMemo(
    () => citas.find((cita) => cita.id === selectedCitaId) || null,
    [citas, selectedCitaId],
  )

  const handleFieldChange = (field) => (event) => {
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
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo guardar la nota.',
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
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo finalizar la nota.',
      )
    } finally {
      setFinalizing(false)
    }
  }

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
              <div className="grid gap-4 md:grid-cols-2">
                <Input
                  label="Estado de ánimo (1-10)"
                  type="number"
                  min="1"
                  max="10"
                  value={form.estadoAnimo}
                  onChange={handleFieldChange('estadoAnimo')}
                />
                <Input
                  label="Nivel de ansiedad (1-10)"
                  type="number"
                  min="1"
                  max="10"
                  value={form.nivelAnsiedad}
                  onChange={handleFieldChange('nivelAnsiedad')}
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Resumen de sesión
                </label>
                <textarea
                  value={form.resumenSesion}
                  onChange={handleFieldChange('resumenSesion')}
                  rows={5}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
                  placeholder="Describe el contenido principal de la sesión."
                />
              </div>

              <Input
                label="Técnicas usadas"
                hint="Separar por comas. Ej.: reestructuración cognitiva, psicoeducación"
                value={form.tecnicasUsadasText}
                onChange={handleFieldChange('tecnicasUsadasText')}
              />

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Avance de objetivos
                </label>
                <textarea
                  value={form.avanceObjetivos}
                  onChange={handleFieldChange('avanceObjetivos')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
                  placeholder="Resume avances observados respecto al plan terapéutico."
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Tareas asignadas
                </label>
                <textarea
                  value={form.tareasAsignadas}
                  onChange={handleFieldChange('tareasAsignadas')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
                  placeholder="Tareas o compromisos acordados para el paciente."
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-700">
                  Plan para próxima sesión
                </label>
                <textarea
                  value={form.planProximaSesion}
                  onChange={handleFieldChange('planProximaSesion')}
                  rows={4}
                  className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
                  placeholder="Define el enfoque o temas de seguimiento."
                />
              </div>

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
                      value={form.nivelRiesgo}
                      onChange={handleFieldChange('nivelRiesgo')}
                      className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
                    >
                      {RISK_OPTIONS.map((option) => (
                        <option key={option || 'none'} value={option}>
                          {option || 'Seleccionar nivel'}
                        </option>
                      ))}
                    </select>
                  </div>
                )}
              </div>

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
