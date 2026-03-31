import { useMemo, useState } from 'react'
import { Search, Receipt, ShieldCheck, UserRound, ChevronDown } from 'lucide-react'
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
import { pagosApi } from '../../api/pagosApi'
import { pacientesApi } from '../../api/pacientesApi'

// ============================================================================
// NORMALIZADORES ROBUSTOS
// ============================================================================

function normalizePagos(data) {
  if (!data) return []
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.items)) return data.items
  if (Array.isArray(data?.data)) return data.data
  if (data && typeof data === 'object') return [data]
  return []
}

function getPacienteId(item) {
  if (!item) return null
  return item?.id || item?.pacienteId || item?.paciente?.id || null
}

function getPacienteNombre(item) {
  if (!item) return 'Paciente sin nombre'
  return (
    item?.nombreCompleto ||
    item?.nombre ||
    item?.paciente?.nombreCompleto ||
    item?.paciente?.nombre ||
    `${item?.nombres || ''} ${item?.apellidos || ''}`.trim() ||
    `${item?.paciente?.nombres || ''} ${item?.paciente?.apellidos || ''}`.trim() ||
    'Paciente sin nombre'
  )
}

function getPagoKey(item, index) {
  if (!item) return `pago-${index}`
  return item?.id || item?.pagoId || item?.citaId || `pago-${index}`
}

function getCitaId(item) {
  if (!item) return null
  return item?.citaId || item?.cita?.id || null
}

function getEstado(item) {
  if (!item) return 'Pendiente'
  const estado = item?.estado || item?.status || item?.resultado || 'Pendiente'
  return String(estado).charAt(0).toUpperCase() + String(estado).slice(1)
}

function getMonto(item) {
  if (!item) return 'No registrado'
  const monto = item?.monto ?? item?.amount ?? item?.total
  if (monto === null || monto === undefined) return 'No registrado'
  return monto
}

function getFecha(item) {
  if (!item) return 'Sin fecha'
  const fecha = item?.fechaPago || item?.fecha || item?.createdAt
  if (!fecha) return 'Sin fecha'
  try {
    return new Date(fecha).toLocaleDateString('es-PE')
  } catch {
    return String(fecha)
  }
}

function getDetalle(item) {
  if (!item) return 'Sin detalle'
  return item?.metodoPago || item?.metodo || item?.detalle || 'Sin detalle'
}

// ============================================================================
// COMPONENTE PRINCIPAL
// ============================================================================

export default function PagoPage() {
  // ─────────────────────────────────────────────────────────────────────────
  // ESTADOS
  // ─────────────────────────────────────────────────────────────────────────

  const [patientQuery, setPatientQuery] = useState('')
  const [patientResults, setPatientResults] = useState([])
  const [selectedPatient, setSelectedPatient] = useState(null)

  const [advancedCitaCode, setAdvancedCitaCode] = useState('')
  const [showAdvanced, setShowAdvanced] = useState(false)

  const [items, setItems] = useState([])
  const [formsByCita, setFormsByCita] = useState({})
  const [searchPatientsLoading, setSearchPatientsLoading] = useState(false)
  const [searchPaymentsLoading, setSearchPaymentsLoading] = useState(false)
  const [searchByCitaLoading, setSearchByCitaLoading] = useState(false)
  const [actionLoading, setActionLoading] = useState({})
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  // ─────────────────────────────────────────────────────────────────────────
  // COMPUTED
  // ─────────────────────────────────────────────────────────────────────────

  const hasResults = useMemo(() => items.length > 0, [items])

  // ─────────────────────────────────────────────────────────────────────────
  // FUNCIONES DE FORMULARIO
  // ─────────────────────────────────────────────────────────────────────────

  const updateForm = (citaId, field, value) => {
    if (!citaId) return
    setFormsByCita((prev) => ({
      ...prev,
      [citaId]: {
        monto: prev[citaId]?.monto ?? '',
        metodoPago: prev[citaId]?.metodoPago ?? 'Yape',
        ...prev[citaId],
        [field]: value,
      },
    }))
  }

  const getForm = (citaId) => {
    if (!citaId) return { monto: '', metodoPago: 'Yape' }
    return formsByCita[citaId] || { monto: '', metodoPago: 'Yape' }
  }

  // ─────────────────────────────────────────────────────────────────────────
  // API CALLS
  // ─────────────────────────────────────────────────────────────────────────

  const handleSearchPatients = async () => {
    const query = patientQuery.trim()
    if (!query) {
      setError('Escribe el nombre o documento del paciente')
      return
    }

    setSearchPatientsLoading(true)
    setError('')
    setSuccessMessage('')
    setPatientResults([])
    setSelectedPatient(null)
    setItems([])

    try {
      const data = await pacientesApi.buscar(query)
      const normalized = Array.isArray(data) ? data : []

      if (normalized.length === 0) {
        setError('No se encontraron pacientes con ese criterio')
        setPatientResults([])
      } else {
        setPatientResults(normalized)
        setSuccessMessage(`Se encontraron ${normalized.length} paciente(s)`)
      }
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar pacientes. Intenta nuevamente.'
      )
      setPatientResults([])
    } finally {
      setSearchPatientsLoading(false)
    }
  }

  const loadPaymentsByPatient = async (patient) => {
    if (!patient) {
      setError('No se pudo identificar el paciente seleccionado')
      return
    }

    const patientId = getPacienteId(patient)
    if (!patientId) {
      setError('El paciente seleccionado no tiene ID válido')
      return
    }

    setSelectedPatient(patient)
    setSearchPaymentsLoading(true)
    setError('')
    setSuccessMessage('')

    try {
      const data = await pagosApi.getByPaciente(patientId)
      const normalized = normalizePagos(data)
      setItems(normalized)

      if (normalized.length === 0) {
        setSuccessMessage('No hay pagos registrados para este paciente')
      } else {
        setSuccessMessage(`Se cargaron ${normalized.length} pago(s)`)
      }
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar pagos por paciente. Intenta nuevamente.'
      )
    } finally {
      setSearchPaymentsLoading(false)
    }
  }

  const handleSearchByCita = async () => {
    const code = advancedCitaCode.trim()
    if (!code) {
      setError('Escribe el código de la cita')
      return
    }

    setSearchByCitaLoading(true)
    setError('')
    setSuccessMessage('')
    setSelectedPatient(null)
    setPatientResults([])

    try {
      const data = await pagosApi.getByCita(code)
      const normalized = normalizePagos(data)
      setItems(normalized)

      if (normalized.length === 0) {
        setError('No se encontraron pagos con ese código de cita')
      } else {
        setSuccessMessage(`Se encontró 1 pago para esa cita`)
      }
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar por código de cita. Intenta nuevamente.'
      )
    } finally {
      setSearchByCitaLoading(false)
    }
  }

  const reloadCurrentView = async () => {
    if (selectedPatient) {
      await loadPaymentsByPatient(selectedPatient)
      return
    }

    if (advancedCitaCode.trim()) {
      await handleSearchByCita()
    }
  }

  const runAction = async (key, fn) => {
    setActionLoading((prev) => ({ ...prev, [key]: true }))
    setError('')
    setSuccessMessage('')

    try {
      await fn()
      setSuccessMessage('Acción completada exitosamente')
      await reloadCurrentView()
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo completar la acción. Intenta nuevamente.'
      )
    } finally {
      setActionLoading((prev) => ({ ...prev, [key]: false }))
    }
  }

  const handleRegistrar = async (item) => {
    if (!item) {
      setError('No se pudo identificar el pago a registrar')
      return
    }

    const citaId = getCitaId(item)
    if (!citaId) {
      setError('No se encontró la cita asociada a este pago')
      return
    }

    const form = getForm(citaId)
    if (!form.monto || Number(form.monto) <= 0) {
      setError('Ingresa un monto válido (mayor a 0) antes de registrar')
      return
    }

    await runAction(`registrar-${citaId}`, async () => {
      await pagosApi.registrar({
        citaId,
        monto: Number(form.monto),
        metodoPago: form.metodoPago,
      })
    })
  }

  const handleExonerar = async (item) => {
    if (!item) {
      setError('No se pudo identificar el pago a exonerar')
      return
    }

    const citaId = getCitaId(item)
    if (!citaId) {
      setError('No se encontró la cita asociada a este pago')
      return
    }

    await runAction(`exonerar-${citaId}`, async () => {
      await pagosApi.exonerar({
        citaId,
        motivo: 'Exoneración manual desde panel',
      })
    })
  }

  // ─────────────────────────────────────────────────────────────────────────
  // RENDER
  // ─────────────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Pagos"
        description="Busca pagos por paciente y gestiona cada registro de forma independiente."
      />

      <div className="grid gap-6 xl:grid-cols-[1fr_1fr]">
        {/* ─────────────────────────────────────────────────────────────────────────
            PANEL DE BÚSQUEDA
            ───────────────────────────────────────────────────────────────────────── */}

        <Card>
          <CardHeader>
            <CardTitle>Buscar paciente</CardTitle>
            <CardDescription>
              Escribe nombre, documento o correo. No mostramos IDs técnicos al usuario.
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-4">
            {error && (
              <div className="flex items-start gap-3 rounded-2xl border border-rose-200 bg-rose-50 p-4 text-rose-700">
                <div className="mt-0.5 h-5 w-5 -flex-shrink-0 rounded-full bg-rose-200" />
                <p className="text-sm font-medium">{error}</p>
              </div>
            )}

            {successMessage && (
              <div className="flex items-start gap-3 rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-emerald-700">
                <div className="mt-0.5 h-5 w-5 -flex-shrink-0 rounded-full bg-emerald-200" />
                <p className="text-sm font-medium">{successMessage}</p>
              </div>
            )}

            <div className="flex flex-col gap-3 md:flex-row">
              <Input
                label="Paciente"
                placeholder="Ej. Luciana, 12345678 o correo@demo.com"
                value={patientQuery}
                onChange={(e) => setPatientQuery(e.target.value)}
                className="flex-1"
              />
              <div className="md:self-end">
                <Button onClick={handleSearchPatients} loading={searchPatientsLoading}>
                  <Search className="h-4 w-4" />
                  Buscar paciente
                </Button>
              </div>
            </div>

            {patientResults.length > 0 && (
              <div className="space-y-3">
                <p className="text-sm font-medium text-slate-700">Resultados</p>

                {patientResults.map((patient, index) => {
                  const patientId = getPacienteId(patient)
                  const isSelected =
                    selectedPatient && getPacienteId(selectedPatient) === patientId

                  return (
                    <button
                      key={patientId || index}
                      onClick={() => loadPaymentsByPatient(patient)}
                      className={[
                        'w-full rounded-2xl border p-4 text-left transition',
                        isSelected
                          ? 'border-slate-900 bg-slate-900 text-white'
                          : 'border-slate-200 bg-white hover:bg-slate-50',
                      ].join(' ')}
                    >
                      <div className="flex items-start gap-3">
                        <div className="mt-0.5 flex h-10 w-10 items-center justify-center rounded-2xl bg-slate-100 text-slate-700">
                          <UserRound className="h-5 w-5" />
                        </div>

                        <div className="min-w-0">
                          <p className="font-medium">{getPacienteNombre(patient)}</p>
                          <p
                            className={[
                              'mt-1 text-sm truncate',
                              isSelected ? 'text-slate-200' : 'text-slate-500',
                            ].join(' ')}
                          >
                            {patient?.documento ||
                              patient?.dni ||
                              patient?.email ||
                              'Sin dato adicional'}
                          </p>
                        </div>
                      </div>
                    </button>
                  )
                })}
              </div>
            )}

            <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
              <button
                type="button"
                onClick={() => setShowAdvanced((prev) => !prev)}
                className="flex w-full items-center justify-between text-left"
              >
                <div>
                  <p className="font-medium text-slate-900">Búsqueda avanzada</p>
                  <p className="mt-1 text-sm text-slate-500">
                    Solo si necesitas buscar por código de cita.
                  </p>
                </div>
                <ChevronDown
                  className={[
                    'h-5 w-5 -flex-shrink-0 text-slate-500 transition',
                    showAdvanced ? 'rotate-180' : '',
                  ].join(' ')}
                />
              </button>

              {showAdvanced && (
                <div className="mt-4 space-y-3">
                  <Input
                    label="Código de cita"
                    placeholder="Pegar código interno solo si es necesario"
                    value={advancedCitaCode}
                    onChange={(e) => setAdvancedCitaCode(e.target.value)}
                  />

                  <Button
                    variant="secondary"
                    onClick={handleSearchByCita}
                    loading={searchByCitaLoading}
                  >
                    <Search className="h-4 w-4" />
                    Buscar por cita
                  </Button>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* ─────────────────────────────────────────────────────────────────────────
            PANEL DE RESULTADOS
            ───────────────────────────────────────────────────────────────────────── */}

        <Card>
          <CardHeader>
            <CardTitle>Resultados</CardTitle>
            <CardDescription>Cada pago se gestiona de forma independiente.</CardDescription>
          </CardHeader>

          <CardContent>
            {searchPaymentsLoading || searchByCitaLoading ? (
              <p className="text-sm text-slate-500">Cargando resultados...</p>
            ) : !hasResults ? (
              <EmptyState
                title="Sin pagos cargados"
                description="Selecciona un paciente o usa la búsqueda avanzada por cita."
              />
            ) : (
              <div className="space-y-4">
                {items.map((item, index) => {
                  const key = getPagoKey(item, index)
                  const citaId = getCitaId(item)
                  const form = getForm(citaId)

                  return (
                    <div key={key} className="rounded-2xl border border-slate-200 p-4">
                      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                        <div className="min-w-0">
                          <p className="font-medium text-slate-900">
                            {item?.pacienteNombre ||
                              getPacienteNombre(selectedPatient || item || {})}
                          </p>
                          <p className="mt-1 text-sm text-slate-500">
                            Fecha: {getFecha(item)}
                          </p>
                          <p className="mt-1 text-sm text-slate-500">
                            Detalle: {getDetalle(item)}
                          </p>
                          <p className="mt-1 text-sm text-slate-500">
                            Monto actual: {getMonto(item)}
                          </p>
                        </div>

                        <StatusBadge status={getEstado(item)}>
                          {getEstado(item)}
                        </StatusBadge>
                      </div>

                      <div className="mt-4 grid gap-3 md:grid-cols-[1fr_1fr_auto_auto]">
                        <Input
                          label="Monto"
                          type="number"
                          placeholder="Ej. 120"
                          value={form.monto}
                          onChange={(e) => updateForm(citaId, 'monto', e.target.value)}
                        />

                        <div className="space-y-2">
                          <label className="block text-sm font-medium text-slate-700">
                            Método de pago
                          </label>
                          <select
                            value={form.metodoPago}
                            onChange={(e) =>
                              updateForm(citaId, 'metodoPago', e.target.value)
                            }
                            className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
                          >
                            <option value="Yape">Yape</option>
                            <option value="Plin">Plin</option>
                            <option value="Efectivo">Efectivo</option>
                            <option value="Tarjeta">Tarjeta</option>
                            <option value="Transferencia">Transferencia</option>
                          </select>
                        </div>

                        <div className="md:self-end">
                          <Button
                            onClick={() => handleRegistrar(item)}
                            loading={!!actionLoading[`registrar-${citaId}`]}
                          >
                            <Receipt className="h-4 w-4" />
                            Registrar
                          </Button>
                        </div>

                        <div className="md:self-end">
                          <Button
                            variant="secondary"
                            onClick={() => handleExonerar(item)}
                            loading={!!actionLoading[`exonerar-${citaId}`]}
                          >
                            <ShieldCheck className="h-4 w-4" />
                            Exonerar
                          </Button>
                        </div>
                      </div>
                    </div>
                  )
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}