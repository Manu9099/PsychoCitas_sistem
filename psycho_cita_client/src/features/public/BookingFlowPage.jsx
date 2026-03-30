import { useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  CalendarDays,
  Clock3,
  UserRound,
  CreditCard,
  ArrowRight,
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

function normalizeSlots(data) {
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.slots)) return data.slots
  if (Array.isArray(data?.horarios)) return data.horarios
  if (Array.isArray(data?.data)) return data.data
  return []
}

function getSlotLabel(slot) {
  return (
    slot?.hora ||
    slot?.horaInicio ||
    slot?.inicio ||
    slot?.label ||
    'Horario disponible'
  )
}

function getSlotValue(slot) {
  return (
    slot?.horaInicio ||
    slot?.inicio ||
    slot?.hora ||
    slot?.value ||
    ''
  )
}

function getCheckoutUrl(data) {
  return (
    data?.checkoutUrl ||
    data?.url ||
    data?.paymentUrl ||
    data?.redirectUrl ||
    ''
  )
}

export default function BookingFlowPage() {
  const navigate = useNavigate()

  const [step, setStep] = useState(1)
  const [loadingSlots, setLoadingSlots] = useState(false)
  const [creatingBooking, setCreatingBooking] = useState(false)
  const [creatingCheckout, setCreatingCheckout] = useState(false)
  const [slots, setSlots] = useState([])
  const [selectedSlot, setSelectedSlot] = useState(null)
  const [createdBooking, setCreatedBooking] = useState(null)
  const [error, setError] = useState('')

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

  const canSearchSlots = useMemo(() => {
    return form.psicologoId.trim() && form.fecha.trim()
  }, [form.psicologoId, form.fecha])

  const canCreateBooking = useMemo(() => {
    return (
      selectedSlot &&
      form.nombres.trim() &&
      form.apellidos.trim() &&
      form.documento.trim() &&
      form.email.trim()
    )
  }, [selectedSlot, form])

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  const handleSearchSlots = async (e) => {
    e.preventDefault()
    if (!canSearchSlots) return

    setLoadingSlots(true)
    setError('')
    setSelectedSlot(null)

    try {
      const data = await publicApi.getDisponibilidad({
        psicologoId: form.psicologoId,
        fecha: form.fecha,
        duracionMinutos: Number(form.duracionMinutos) || 50,
      })
      setSlots(normalizeSlots(data))
      setStep(2)
    } catch (err) {
      setSlots([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo cargar la disponibilidad'
      )
    } finally {
      setLoadingSlots(false)
    }
  }

  const handleCreateBooking = async () => {
    if (!canCreateBooking) return

    setCreatingBooking(true)
    setError('')

    try {
      const payload = {
        psicologoId: form.psicologoId,
        fecha: form.fecha,
        horaInicio: getSlotValue(selectedSlot),
        duracionMinutos: Number(form.duracionMinutos) || 50,
        nombres: form.nombres,
        apellidos: form.apellidos,
        documento: form.documento,
        email: form.email,
        telefono: form.telefono,
        motivoConsulta: form.motivoConsulta,
        modalidad: form.modalidad,
      }

      const data = await publicApi.crearCita(payload)
      setCreatedBooking(data)
      setStep(3)
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo crear la cita'
      )
    } finally {
      setCreatingBooking(false)
    }
  }

  const handleCreateCheckout = async () => {
    if (!createdBooking) return

    setCreatingCheckout(true)
    setError('')

    try {
      const citaId =
        createdBooking?.citaId ||
        createdBooking?.id ||
        createdBooking?.bookingId

      const payload = {
        citaId,
        pacienteEmail: form.email,
        pacienteNombre: `${form.nombres} ${form.apellidos}`.trim(),
      }

      const data = await publicApi.crearCheckout(payload)
      const checkoutUrl = getCheckoutUrl(data)

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
          'No se pudo iniciar el checkout'
      )
    } finally {
      setCreatingCheckout(false)
    }
  }

  return (
    <div className="min-h-screen bg-slate-50 px-4 py-8 md:px-6">
      <div className="mx-auto max-w-6xl space-y-6">
        <PageHeader
          eyebrow="Reserva pública"
          title="Agenda tu cita"
          description="Selecciona fecha, horario y completa tus datos para continuar con el pago."
        />

        <div className="grid gap-4 md:grid-cols-3">
          <Card className={step >= 1 ? 'border-slate-300' : ''}>
            <CardContent className="flex items-center gap-3 p-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-slate-100">
                <CalendarDays className="h-5 w-5 text-slate-700" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-900">1. Fecha</p>
                <p className="text-sm text-slate-500">Busca disponibilidad</p>
              </div>
            </CardContent>
          </Card>

          <Card className={step >= 2 ? 'border-slate-300' : ''}>
            <CardContent className="flex items-center gap-3 p-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-slate-100">
                <Clock3 className="h-5 w-5 text-slate-700" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-900">2. Horario</p>
                <p className="text-sm text-slate-500">Elige tu cita</p>
              </div>
            </CardContent>
          </Card>

          <Card className={step >= 3 ? 'border-slate-300' : ''}>
            <CardContent className="flex items-center gap-3 p-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-slate-100">
                <CreditCard className="h-5 w-5 text-slate-700" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-900">3. Pago</p>
                <p className="text-sm text-slate-500">Continúa al checkout</p>
              </div>
            </CardContent>
          </Card>
        </div>

        {error ? (
          <Card>
            <CardContent>
              <p className="text-sm text-rose-600">{error}</p>
            </CardContent>
          </Card>
        ) : null}

        <div className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Disponibilidad</CardTitle>
                <CardDescription>
                  Completa los datos base para consultar los horarios.
                </CardDescription>
              </CardHeader>

              <CardContent>
                <form onSubmit={handleSearchSlots} className="grid gap-4 md:grid-cols-2">
                  <Input
                    label="Psicólogo ID"
                    name="psicologoId"
                    value={form.psicologoId}
                    onChange={handleChange}
                    placeholder="GUID del psicólogo"
                  />

                  <Input
                    label="Fecha"
                    name="fecha"
                    type="date"
                    value={form.fecha}
                    onChange={handleChange}
                  />

                  <Input
                    label="Duración (min)"
                    name="duracionMinutos"
                    type="number"
                    value={form.duracionMinutos}
                    onChange={handleChange}
                  />

                  <div className="flex items-end">
                    <Button type="submit" loading={loadingSlots} className="w-full">
                      Buscar horarios
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Datos del paciente</CardTitle>
                <CardDescription>
                  Información mínima para generar la cita.
                </CardDescription>
              </CardHeader>

              <CardContent className="grid gap-4 md:grid-cols-2">
                <Input
                  label="Nombres"
                  name="nombres"
                  value={form.nombres}
                  onChange={handleChange}
                />
                <Input
                  label="Apellidos"
                  name="apellidos"
                  value={form.apellidos}
                  onChange={handleChange}
                />
                <Input
                  label="Documento"
                  name="documento"
                  value={form.documento}
                  onChange={handleChange}
                />
                <Input
                  label="Correo"
                  name="email"
                  type="email"
                  value={form.email}
                  onChange={handleChange}
                />
                <Input
                  label="Teléfono"
                  name="telefono"
                  value={form.telefono}
                  onChange={handleChange}
                />
                <div className="space-y-2">
                  <label className="block text-sm font-medium text-slate-700">
                    Modalidad
                  </label>
                  <select
                    name="modalidad"
                    value={form.modalidad}
                    onChange={handleChange}
                    className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
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
                    className="mt-2 w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
                    placeholder="Cuéntanos brevemente el motivo de tu consulta"
                  />
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Horarios disponibles</CardTitle>
                <CardDescription>
                  Selecciona el horario que prefieras.
                </CardDescription>
              </CardHeader>

              <CardContent>
                {slots.length === 0 ? (
                  <EmptyState
                    title="Sin horarios cargados"
                    description="Primero busca disponibilidad para ver los espacios libres."
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
                          onClick={() => {
                            setSelectedSlot(slot)
                            setStep(2)
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
          </div>

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
                  <p className="text-sm text-slate-500">Fecha</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {form.fecha || 'No seleccionada'}
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Horario</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {selectedSlot ? getSlotLabel(selectedSlot) : 'No seleccionado'}
                  </p>
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-sm text-slate-500">Paciente</p>
                  <p className="mt-1 font-medium text-slate-900">
                    {[form.nombres, form.apellidos].filter(Boolean).join(' ') || 'Sin datos'}
                  </p>
                </div>

                <div className="flex items-center gap-2">
                  <StatusBadge status={selectedSlot ? 'success' : 'pending'}>
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

            {createdBooking ? (
              <Card>
                <CardHeader>
                  <CardTitle>Cita creada</CardTitle>
                  <CardDescription>
                    Ya puedes pasar al checkout.
                  </CardDescription>
                </CardHeader>

                <CardContent className="space-y-3">
                  <div className="rounded-2xl border border-slate-200 p-4">
                    <p className="text-sm text-slate-500">Cita ID</p>
                    <p className="mt-1 break-all font-medium text-slate-900">
                      {createdBooking?.citaId || createdBooking?.id || 'No disponible'}
                    </p>
                  </div>
                </CardContent>
              </Card>
            ) : null}
          </div>
        </div>
      </div>
    </div>
  )
}