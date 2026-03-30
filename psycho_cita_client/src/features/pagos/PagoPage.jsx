import { useState } from 'react'
import { Search, Receipt, ShieldCheck } from 'lucide-react'
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


function normalizePagos(data) {
  if (Array.isArray(data)) return data
  if (Array.isArray(data?.items)) return data.items
  if (data && typeof data === 'object') return [data]
  return []
}

function getPagoLabel(item) {
  return (
    item?.estado ||
    item?.status ||
    item?.resultado ||
    'Pendiente'
  )
}

export default function PagoPage() {
  const [citaId, setCitaId] = useState('')
  const [pacienteId, setPacienteId] = useState('')
  const [monto, setMonto] = useState('')
  const [metodo, setMetodo] = useState('Yape')
  const [loading, setLoading] = useState(false)
  const [items, setItems] = useState([])
  const [error, setError] = useState('')

  const buscarPorCita = async () => {
    if (!citaId.trim()) return
    setLoading(true)
    setError('')
    try {
      const data = await pagosApi.getByCita(citaId.trim())
      setItems(normalizePagos(data))
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar el pago por cita'
      )
    } finally {
      setLoading(false)
    }
  }

  const buscarPorPaciente = async () => {
    if (!pacienteId.trim()) return
    setLoading(true)
    setError('')
    try {
      const data = await pagosApi.getByPaciente(pacienteId.trim())
      setItems(normalizePagos(data))
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar pagos por paciente'
      )
    } finally {
      setLoading(false)
    }
  }

  const registrarPago = async () => {
    if (!citaId.trim() || !monto.trim()) return
    setLoading(true)
    setError('')

    try {
      await pagosApi.registrar({
        citaId: citaId.trim(),
        monto: Number(monto),
        metodoPago: metodo,
      })
      await buscarPorCita()
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo registrar el pago'
      )
      setLoading(false)
    }
  }

  const exonerarPago = async () => {
    if (!citaId.trim()) return
    setLoading(true)
    setError('')

    try {
      await pagosApi.exonerar({
        citaId: citaId.trim(),
        motivo: 'Exoneración manual desde panel',
      })
      await buscarPorCita()
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo exonerar el pago'
      )
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Pagos"
        description="Consulta, registra o exonera pagos desde el panel interno."
      />

      <div className="grid gap-6 xl:grid-cols-[1fr_1fr]">
        <Card>
          <CardHeader>
            <CardTitle>Consulta</CardTitle>
            <CardDescription>
              Busca pagos por cita o por paciente.
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-4">
            <Input
              label="Cita ID"
              value={citaId}
              onChange={(e) => setCitaId(e.target.value)}
              placeholder="GUID de la cita"
            />

            <div className="flex gap-3">
              <Button onClick={buscarPorCita} loading={loading}>
                <Search className="h-4 w-4" />
                Buscar por cita
              </Button>
            </div>

            <Input
              label="Paciente ID"
              value={pacienteId}
              onChange={(e) => setPacienteId(e.target.value)}
              placeholder="GUID del paciente"
            />

            <div className="flex gap-3">
              <Button variant="secondary" onClick={buscarPorPaciente} loading={loading}>
                <Search className="h-4 w-4" />
                Buscar por paciente
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Acciones</CardTitle>
            <CardDescription>
              Registro manual o exoneración.
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-4">
            <Input
              label="Monto"
              type="number"
              value={monto}
              onChange={(e) => setMonto(e.target.value)}
              placeholder="Ej. 120"
            />

            <div className="space-y-2">
              <label className="block text-sm font-medium text-slate-700">
                Método de pago
              </label>
              <select
                value={metodo}
                onChange={(e) => setMetodo(e.target.value)}
                className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-violet-500"
              >
                <option value="Yape">Yape</option>
                <option value="Plin">Plin</option>
                <option value="Efectivo">Efectivo</option>
                <option value="Tarjeta">Tarjeta</option>
                <option value="Transferencia">Transferencia</option>
              </select>
            </div>

            <div className="flex flex-wrap gap-3">
              <Button onClick={registrarPago} loading={loading}>
                <Receipt className="h-4 w-4" />
                Registrar pago
              </Button>

              <Button variant="secondary" onClick={exonerarPago} loading={loading}>
                <ShieldCheck className="h-4 w-4" />
                Exonerar
              </Button>
            </div>

            {error ? <p className="text-sm text-rose-600">{error}</p> : null}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Resultados</CardTitle>
          <CardDescription>Listado de pagos encontrados.</CardDescription>
        </CardHeader>

        <CardContent>
          {items.length === 0 ? (
            <EmptyState
              title="Sin pagos cargados"
              description="Haz una búsqueda para ver resultados."
            />
          ) : (
            <div className="grid gap-4">
              {items.map((item, index) => (
                <div
                  key={item?.id || index}
                  className="rounded-2xl border border-slate-200 p-4"
                >
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div>
                      <p className="font-medium text-slate-900">
                        Pago {item?.id || index + 1}
                      </p>
                      <p className="mt-1 text-sm text-slate-500">
                        Cita: {item?.citaId || 'No disponible'}
                      </p>
                      <p className="mt-1 text-sm text-slate-500">
                        Monto: {item?.monto ?? 'No disponible'}
                      </p>
                    </div>

                    <StatusBadge status={getPagoLabel(item)}>
                      {getPagoLabel(item)}
                    </StatusBadge>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}