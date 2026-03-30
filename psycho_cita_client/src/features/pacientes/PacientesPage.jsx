import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Search, UserRound, Mail, FileText } from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Input from '../../components/ui/Input'
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
import { pacientesApi } from '../../api/pacientesApi'

function getPacienteName(item) {
  return (
    item?.nombreCompleto ||
    item?.nombre ||
    `${item?.nombres || ''} ${item?.apellidos || ''}`.trim() ||
    'Paciente sin nombre'
  )
}

function getPacienteSubtitle(item) {
  return (
    item?.documento ||
    item?.dni ||
    item?.numeroDocumento ||
    item?.email ||
    'Sin dato adicional'
  )
}

export default function PacientesPage() {
  const navigate = useNavigate()

  const [termino, setTermino] = useState('')
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(false)
  const [searched, setSearched] = useState(false)
  const [error, setError] = useState('')

  const handleSearch = async (e) => {
    e.preventDefault()

    const query = termino.trim()

    if (!query) {
      setItems([])
      setSearched(false)
      setError('')
      return
    }

    setLoading(true)
    setError('')
    setSearched(true)

    try {
      const data = await pacientesApi.buscar(query)
      setItems(Array.isArray(data) ? data : [])
    } catch (err) {
      setItems([])
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo buscar pacientes'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title="Pacientes"
        description="Busca pacientes por nombre, documento o correo para revisar su información."
      />

      <Card>
        <CardHeader>
          <CardTitle>Búsqueda</CardTitle>
          <CardDescription>
            Ingresa un nombre, documento o correo.
          </CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSearch} className="flex flex-col gap-3 md:flex-row">
            <Input
              id="buscar-paciente"
              placeholder="Ej. Luciana, 12345678 o correo@demo.com"
              value={termino}
              onChange={(e) => setTermino(e.target.value)}
              className="flex-1"
            />

            <Button type="submit" loading={loading} className="md:self-end">
              <Search className="h-4 w-4" />
              Buscar
            </Button>
          </form>

          {error ? (
            <p className="mt-4 text-sm text-rose-600">{error}</p>
          ) : null}
        </CardContent>
      </Card>

      {!searched && !loading ? (
        <EmptyState
          title="Busca un paciente"
          description="Todavía no has realizado una búsqueda. Empieza escribiendo un nombre, documento o correo."
        />
      ) : null}

      {searched && !loading && !error && items.length === 0 ? (
        <EmptyState
          title="No se encontraron pacientes"
          description="No hubo resultados para tu búsqueda. Prueba con otro nombre o documento."
        />
      ) : null}

      {items.length > 0 ? (
        <div className="grid gap-4">
          {items.map((item, index) => {
            const pacienteId = item?.id || item?.pacienteId || index
            const nombre = getPacienteName(item)
            const subtitle = getPacienteSubtitle(item)

            return (
              <Card
                key={pacienteId}
                className="transition hover:-translate-y-0.5 hover:shadow-md"
              >
                <CardContent className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                  <div className="flex min-w-0 items-start gap-4">
                    <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-slate-100">
                      <UserRound className="h-5 w-5 text-slate-700" />
                    </div>

                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2">
                        <p className="truncate text-base font-semibold text-slate-900">
                          {nombre}
                        </p>

                        <StatusBadge status="activo">Paciente</StatusBadge>
                      </div>

                      <div className="mt-2 space-y-1 text-sm text-slate-500">
                        <div className="flex items-center gap-2">
                          <FileText className="h-4 w-4" />
                          <span className="truncate">{subtitle}</span>
                        </div>

                        {item?.email ? (
                          <div className="flex items-center gap-2">
                            <Mail className="h-4 w-4" />
                            <span className="truncate">{item.email}</span>
                          </div>
                        ) : null}
                      </div>
                    </div>
                  </div>

                  <div className="flex shrink-0 items-center gap-2">
                    <Button
                      variant="secondary"
                      onClick={() => navigate(`/app/pacientes/${pacienteId}`)}
                    >
                      Ver detalle
                    </Button>
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