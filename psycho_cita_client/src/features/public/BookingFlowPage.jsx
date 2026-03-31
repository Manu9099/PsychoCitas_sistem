import { useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  CalendarDays,
  Clock3,
  CreditCard,
  ArrowRight,
  UserRound,
  AlertCircle,
} from 'lucide-react'
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
import { publicApi } from '../../api/publicApi'

// ============================================================================
// DATOS TEMPORALES (REEMPLAZAR CON API CUANDO SEA DISPONIBLE)
// ============================================================================

const PSYCHOLOGISTS = [
  { id: '11111111-1111-1111-1111-111111111111', label: 'Dra. Valeria Torres' },
  { id: '22222222-2222-2222-2222-222222222222', label: 'Lic. Andrea Rojas' },
  { id: '33333333-3333-3333-3333-333333333333', label: 'Mg. Camila Vega' },
]

// ============================================================================
// NORMALIZADORES ROBUSTOS
// ============================================================================

function normalizeSlots(data) {
  if (!data) return []
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.slots)) return data.slots
  if (Array.isArray(data?.horarios)) return data.horarios
  if (Array.isArray(data?.data)) return data.data
  return []
}

function getSlotLabel(slot) {
  if (!slot) return 'Horario disponible'
  return (
    slot?.hora ||
    slot?.horaInicio ||
    slot?.inicio ||
    slot?.label ||
    'Horario disponible'
  )
}

function getSlotValue(slot) {
  if (!slot) return ''
  return slot?.horaInicio || slot?.inicio || slot?.hora || slot?.value || ''
}

function getCheckoutUrl(data) {
  if (!data) return ''
  return (
    data?.checkoutUrl ||
    data?.url ||
    data?.paymentUrl ||
    data?.redirectUrl ||
    ''
  )
}

function getCitaId(data) {
  if (!data) return null
  return data?.citaId || data?.id || data?.bookingId || null
}

// ============================================================================
// VALIDADORES
// ============================================================================

function validateEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return regex.test(email)
}

function validateForm(form) {
  const errors = []

  if (!form.nombres?.trim()) errors.push('Nombres requeridos')
  if (!form.apellidos?.trim()) errors.push('Apellidos requeridos')
  if (!form.documento?.trim()) errors.push('Documento requerido')
  if (!form.email?.trim()) errors.push('Correo requerido')
  if (form.email && !validateEmail(form.email)) errors.push('Correo inválido')

  return errors
}

// ============================================================================
// COMPONENTE PRINCIPAL
// ============================================================================

export default function BookingFlowPage() {
  const navigate = useNavigate()

  // ─────────────────────────────────────────────────────────────────────────
  // ESTADOS
  // ─────────────────────────────────────────────────────────────────────────

  const [loadingSlots, setLoadingSlots] = useState(false)
  const [creatingBooking, setCreatingBooking] = useState(false)
  const [creatingCheckout, setCreatingCheckout] = useState(false)
  const [slots, setSlots] = useState([])
  const [selectedSlot, setSelectedSlot] = useState(null)
  const [createdBooking, setCreatedBooking] = useState(null)
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  const [form, setForm] = useState({
    psicologoId: '',
    fecha: '',
    duracionMinutos: 50,
    nombres: '',
    apellidos: '',
    documento: '',
    email: '',
    telefono: '',
    motivoConsulta: '',
    modalidad: 'Virtual',
  })

  // ─────────────────────────────────────────────────────────────────────────
  // COMPUTED
  // ─────────────────────────────────────────────────────────────────────────

  const selectedPsychologist = useMemo(
    () => PSYCHOLOGISTS.find((item) => item.id === form.psicologoId) || null,
    [form.psicologoId]
  )

  const canSearchSlots = useMemo(() => {
    return form.psicologoId.trim() && form.fecha.trim()
  }, [form.psicologoId, form.fecha])

  const formErrors = useMemo(() => {
    return validateForm(form)
  }, [form])

  const canCreateBooking = useMemo(() => {
    return selectedSlot && formErrors.length === 0
  }, [selectedSlot, formErrors])

  // ─────────────────────────────────────────────────────────────────────────
  // HANDLERS
  // ─────────────────────────────────────────────────────────────────────────

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
    setError('')
  }

  const handleSearchSlots = async (e) => {
    e.preventDefault()
    if (!canSearchSlots) {
      setError('Selecciona psicólogo y fecha')
      return
    }

    setLoadingSlots(true)
    setError('')
    setSuccessMessage('')
    setSelectedSlot(null)
    setCreatedBooking(null)

    try {
      const data = await publicApi.getDisponibilidad({
        psicologoId: form.psicologoId,
        fecha: form.fecha,
        duracionMinutos: Number(form.duracionMinutos) || 50,
      })

      const normalized = normalizeSlots(data)

      if (normalized.length === 0) {
        setError('No hay horarios disponibles para esta fecha')
        setSlots([])
      } else {
        setSlots(normalized)
        setSuccessMessage(`Se encontraron ${normalized.length} horario(s)`)
      }
    } catch (err) {
      setSlots([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo cargar la disponibilidad. Intenta nuevamente.'
      )
    } finally {
      setLoadingSlots(false)
    }
  }

  const handleCreateBooking = async () => {
    if (!canCreateBooking) {
      const errors = formErrors.length > 0 ? formErrors : ['Selecciona un horario']
      setError(errors[0])
      return
    }

    setCreatingBooking(true)
    setError('')
    setSuccessMessage('')

    try {
      const payload = {
        psicologoId: form.psicologoId,
        fecha: form.fecha,
        horaInicio: getSlotValue(selectedSlot),
        duracionMinutos: Number(form.duracionMinutos) || 50,
        nombres: form.nombres.trim(),
        apellidos: form.apellidos.trim(),
        documento: form.documento.trim(),
        email: form.email.trim(),
        telefono: form.telefono.trim(),
        motivoConsulta: form.motivoConsulta.trim(),
        modalidad: form.modalidad,
      }

      const data = await publicApi.crearCita(payload)

      if (!data) {
        setError('No se recibió confirmación de la cita')
        return
      }

      setCreatedBooking(data)
      setSuccessMessage('Cita creada exitosamente')
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo crear la cita. Intenta nuevamente.'
      )
    } finally {
      setCreatingBooking(false)
    }
  }

  const handleCreateCheckout = async () => {
    if (!createdBooking) {
      setError('No hay cita creada para continuar')
      return
    }

    setCreatingCheckout(true)
    setError('')
    setSuccessMessage('')

    try {
      const citaId = getCitaId(createdBooking)

      if (!citaId) {
        setError('No se encontró el ID de la cita creada')
        return
      }

      const payload = {
        citaId,
        pacienteEmail: form.email.trim(),
        pacienteNombre: `${form.nombres} ${form.apellidos}`.trim(),
      }

      const data = await publicApi.crearCheckout(payload)

      if (!data) {
        setError('No se pudo iniciar el proceso de pago')
        return
      }

      const checkoutUrl = getCheckoutUrl(data)

      if (!checkoutUrl) {
        setError('No se recibió URL de checkout')
        return
      }

      navigate('/reservar/resultado', {
        state: {
          booking: createdBooking,
          checkout: data,
          checkoutUrl,
        },
      })
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo iniciar el checkout. Intenta nuevamente.'
      )
    } finally {
      setCreatingCheckout(false)
    }
  }

  // ─────────────────────────────────────────────────────────────────────────
  // RENDER
  // ─────────────────────────────────────────────────────────────────────────

  return (
    <div className="min-h-screen bg-slate-50 px-4 py-8 md:px-6">
      <div className="mx-auto max-w-6xl space-y-6">
        <PageHeader
          eyebrow="Reserva pública"
          title="Agenda tu cita"
          description="Selecciona psicólogo, fecha, horario y completa tus datos para continuar con el pago."
        />

        {error && (
          <Card>
            <CardContent>
              <div className="flex items-start gap-3 rounded-2xl border border-rose-200 bg-rose-50 p-4 text-rose-700">
                <AlertCircle className="mt-0.5 h-5 w-5 -flex-shrink-0" />
                <p className="text-sm font-medium">{error}</p>
              </div>
            </CardContent>
          </Card>
        )}

        {successMessage && (
          <Card>
            <CardContent>
              <div className="flex items-start gap-3 rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-emerald-700">
                <div className="mt-0.5 h-5 w-5 -flex-shrink-0 rounded-full bg-emerald-300" />
                <p className="text-sm font-medium">{successMessage}</p>
              </div>
            </CardContent>
          </Card>
        )}

        <div className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
          {/* ─────────────────────────────────────────────────────────────────────────
              COLUMNA IZQUIERDA: FORMULARIO
              ───────────────────────────────────────────────────────────────────────── */}

          <div className="space-y-6">
            {/* Disponibilidad */}
            <Card>
              <CardHeader>
                <CardTitle>Disponibilidad</CardTitle>
                <CardDescription>
                  El usuario elige opciones simples; el sistema usa el ID por detrás.
                </CardDescription>
              </CardHeader>

              <CardContent>
                <form onSubmit={handleSearchSlots} className="grid gap-4 md:grid-cols-2">
                  <div className="space-y-2">
                    <label className="block text-sm font-medium text-slate-700">
                      Psicólogo <span className="text-rose-500">*</span>
                    </label>
                    <select
                      name="psicologoId"
                      value={form.psicologoId}
                      onChange={handleChange}
                      className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                    >
                      <option value="">Selecciona un profesional</option>
                      {PSYCHOLOGISTS.map((item) => (
                        <option key={item.id} value={item.id}>
                          {item.label}
                        </option>
                      ))}
                    </select>
                  </div>

                  <Input
                    label="Fecha"
                    name="fecha"
                    type="date"
                    value={form.fecha}
                    onChange={handleChange}
                    required
                  />

                  <Input
                    label="Duración (min)"
                    name="duracionMinutos"
                    type="number"
                    min="30"
                    max="240"
                    step="10"
                    value={form.duracionMinutos}
                    onChange={handleChange}
                  />

                  <div className="flex items-end">
                    <Button
                      type="submit"
                      loading={loadingSlots}
                      className="w-full"
                    >
                      Buscar horarios
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>

            {/* Horarios Disponibles */}
            <Card>
              <CardHeader>
                <CardTitle>Horarios disponibles</CardTitle>
                <CardDescription>
                  Selecciona el horario que prefieras.
                </CardDescription>
              </CardHeader>

              <CardContent>
                {loadingSlots ? (
                  <p className="text-sm text-slate-500">Cargando horarios...</p>
                ) : slots.length === 0 ? (
                  <EmptyState
                    title="Sin horarios cargados"
                    description="Primero selecciona profesional y fecha para ver disponibilidad."
                  />
                ) : (
                  <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                    {slots.map((slot, index) => {
                      const value = getSlotValue(slot)
                      const label = getSlotLabel(slot)
                      const isSelected = selectedSlot === slot

                      return (
                        <button
                          key={`${value}-${index}`}
                          type="button"
                          onClick={() => {
                            setSelectedSlot(slot)
                            setError('')
                          }}
                          className={[
                            'rounded-2xl border px-4 py-4 text-left transition',
                            isSelected
                              ? 'border-slate-900 bg-slate-900 text-white'
                              : 'border-slate-200 bg-white hover:bg-slate-50',
                          ].join(' ')}
                        >
                          <p className="font-medium">{label}</p>
                          <p
                            className={[
                              'mt-1 text-sm',
                              isSelected ? 'text-slate-200' : 'text-slate-500',
                            ].join(' ')}
                          >
                            Disponible
                          </p>
                        </button>
                      )
                    })}
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Datos del Paciente */}
            <Card>
              <CardHeader>
                <CardTitle>Datos del paciente</CardTitle>
                <CardDescription>
                  Información mínima para generar la cita.
                </CardDescription>
              </CardHeader>

              <CardContent className="grid gap-4 md:grid-cols-2">
                <div>
                  <Input
                    label="Nombres"
                    name="nombres"
                    value={form.nombres}
                    onChange={handleChange}
                    placeholder="Juan"
                    required
                  />
                  {formErrors.some((e) => e.includes('Nombres')) && (
                    <p className="mt-1 text-xs text-rose-600">Nombres requeridos</p>
                  )}
                </div>

                <div>
                  <Input
                    label="Apellidos"
                    name="apellidos"
                    value={form.apellidos}
                    onChange={handleChange}
                    placeholder="Pérez"
                    required
                  />
                  {formErrors.some((e) => e.includes('Apellidos')) && (
                    <p className="mt-1 text-xs text-rose-600">Apellidos requeridos</p>
                  )}
                </div>

                <div>
                  <Input
                    label="Documento"
                    name="documento"
                    value={form.documento}
                    onChange={handleChange}
                    placeholder="12345678"
                    required
                  />
                  {formErrors.some((e) => e.includes('Documento')) && (
                    <p className="mt-1 text-xs text-rose-600">Documento requerido</p>
                  )}
                </div>

                <div>
                  <Input
                    label="Correo"
                    name="email"
                    type="email"
                    value={form.email}
                    onChange={handleChange}
                    placeholder="correo@ejemplo.com"
                    required
                  />
                  {formErrors.some((e) => e.includes('Correo')) && (
                    <p className="mt-1 text-xs text-rose-600">
                      {formErrors.find((e) => e.includes('Correo'))}
                    </p>
                  )}
                </div>

                <Input
                  label="Teléfono"
                  name="telefono"
                  value={form.telefono}
                  onChange={handleChange}
                  placeholder="+51 9 1234567"
                />

                <div className="space-y-2">
                  <label className="block text-sm font-medium text-slate-700">
                    Modalidad
                  </label>
                  <select
                    name="modalidad"
                    value={form.modalidad}
                    onChange={handleChange}
                    className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  >
                    <option value="Virtual">Virtual</option>
                    <option value="Presencial">Presencial</option>
                  </select>
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700">
                    Motivo de consulta
                  </label>
                  <textarea
                    name="motivoConsulta"
                    value={form.motivoConsulta}
                    onChange={handleChange}
                    rows={4}
                    placeholder="Cuéntanos brevemente el motivo de tu consulta"
                    className="mt-2 w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500 focus:ring-1 focus:ring-violet-500"
                  />
                </div>
              </CardContent>
            </Card>
          </div>

          {/* ─────────────────────────────────────────────────────────────────────────
              COLUMNA DERECHA: RESUMEN + BOTONES
              ───────────────────────────────────────────────────────────────────────── */}

          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Resumen</CardTitle>
                <CardDescription>
                  Verifica la selección antes de continuar.
                </CardDescription>
              </CardHeader>

              <CardContent className="space-y-4">
                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Profesional</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {selectedPsychologist?.label || 'No seleccionado'}
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Fecha</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {form.fecha
                      ? new Date(form.fecha).toLocaleDateString('es-PE')
                      : 'No seleccionada'}
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Horario</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {selectedSlot ? getSlotLabel(selectedSlot) : 'No seleccionado'}
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Duración</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {form.duracionMinutos} minutos
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Paciente</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {[form.nombres, form.apellidos].filter(Boolean).join(' ') ||
                      'Sin datos'}
                  </p>
                </div>

                <div className="flex items-center gap-2">
                  <StatusBadge status={selectedSlot ? 'success' : 'pendiente'}>
                    {selectedSlot ? 'Horario elegido' : 'Pendiente'}
                  </StatusBadge>
                </div>

                {!createdBooking ? (
                  <Button
                    className="w-full"
                    onClick={handleCreateBooking}
                    loading={creatingBooking}
                    disabled={!canCreateBooking}
                  >
                    Crear cita
                  </Button>
                ) : (
                  <Button
                    className="w-full"
                    onClick={handleCreateCheckout}
                    loading={creatingCheckout}
                  >
                    Continuar al pago
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                )}
              </CardContent>
            </Card>

            {createdBooking && (
              <Card>
                <CardHeader>
                  <CardTitle>Cita creada</CardTitle>
                  <CardDescription>
                    Ya puedes continuar al checkout.
                  </CardDescription>
                </CardHeader>

                <CardContent>
                  <div className="flex items-center gap-3 rounded-2xl bg-emerald-50 p-4 text-emerald-700">
                    <UserRound className="h-5 w-5 -flex-shrink-0" />
                    <p className="text-sm font-medium">
                      La reserva fue generada correctamente
                    </p>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}