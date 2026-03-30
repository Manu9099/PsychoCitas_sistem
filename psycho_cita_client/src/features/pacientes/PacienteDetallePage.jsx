import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  ArrowLeft,
  Mail,
  Phone,
  FileText,
  UserRound,
  CalendarDays,
} from 'lucide-react'
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
import { pacientesApi } from '../../api/pacientesApi'

function getNombre(data) {
  return (
    data?.nombreCompleto ||
    data?.nombre ||
    `${data?.nombres || ''} ${data?.apellidos || ''}`.trim() ||
    'Paciente sin nombre'
  )
}

function getDocumento(data) {
  return data?.documento || data?.dni || data?.numeroDocumento || 'No registrado'
}

function getEmail(data) {
  return data?.email || 'No registrado'
}

function getTelefono(data) {
  return data?.telefono || data?.celular || data?.phoneNumber || 'No registrado'
}

function getFechaNacimiento(data) {
  return (
    data?.fechaNacimiento ||
    data?.birthDate ||
    data?.fechaNac ||
    'No registrada'
  )
}

export default function PacienteDetallePage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const [paciente, setPaciente] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const loadPaciente = async () => {
      setLoading(true)
      setError('')

      try {
        const data = await pacientesApi.getById(id)
        setPaciente(data)
      } catch (err) {
        setPaciente(null)
        setError(
          err?.response?.data?.message ||
            err?.response?.data?.title ||
            'No se pudo cargar el paciente'
        )
      } finally {
        setLoading(false)
      }
    }

    loadPaciente()
  }, [id])

  if (loading) {
    return (
      <div className="space-y-6">
        <PageHeader
          eyebrow="PsychoCitas"
          title="Detalle de paciente"
          description="Cargando información..."
          actions={
            <Button variant="secondary" onClick={() => navigate('/app/pacientes')}>
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
          }
        />

        <Card>
          <CardContent>
            <p className="text-sm text-slate-500">Cargando paciente...</p>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (error) {
    return (
      <div className="space-y-6">
        <PageHeader
          eyebrow="PsychoCitas"
          title="Detalle de paciente"
          description="No se pudo cargar la información."
          actions={
            <Button variant="secondary" onClick={() => navigate('/app/pacientes')}>
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
          }
        />

        <Card>
          <CardContent>
            <p className="text-sm text-rose-600">{error}</p>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (!paciente) {
    return (
      <div className="space-y-6">
        <PageHeader
          eyebrow="PsychoCitas"
          title="Detalle de paciente"
          description="No hay información disponible."
          actions={
            <Button variant="secondary" onClick={() => navigate('/app/pacientes')}>
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
          }
        />

        <EmptyState
          title="Paciente no encontrado"
          description="No se encontró información para este paciente."
        />
      </div>
    )
  }

  const nombre = getNombre(paciente)
  const documento = getDocumento(paciente)
  const email = getEmail(paciente)
  const telefono = getTelefono(paciente)
  const fechaNacimiento = getFechaNacimiento(paciente)

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="PsychoCitas"
        title={nombre}
        description="Ficha principal del paciente."
        actions={
          <Button variant="secondary" onClick={() => navigate('/app/pacientes')}>
            <ArrowLeft className="h-4 w-4" />
            Volver
          </Button>
        }
      />

      <div className="grid gap-4 xl:grid-cols-[1.2fr_0.8fr]">
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between gap-3">
              <div>
                <CardTitle>Información general</CardTitle>
                <CardDescription>
                  Datos principales del paciente
                </CardDescription>
              </div>

              <StatusBadge status="activo">Activo</StatusBadge>
            </div>
          </CardHeader>

          <CardContent className="grid gap-4 md:grid-cols-2">
            <div className="rounded-2xl bg-slate-50 p-4">
              <div className="mb-2 flex items-center gap-2 text-slate-500">
                <UserRound className="h-4 w-4" />
                <span className="text-sm">Nombre</span>
              </div>
              <p className="font-medium text-slate-900">{nombre}</p>
            </div>

            <div className="rounded-2xl bg-slate-50 p-4">
              <div className="mb-2 flex items-center gap-2 text-slate-500">
                <FileText className="h-4 w-4" />
                <span className="text-sm">Documento</span>
              </div>
              <p className="font-medium text-slate-900">{documento}</p>
            </div>

            <div className="rounded-2xl bg-slate-50 p-4">
              <div className="mb-2 flex items-center gap-2 text-slate-500">
                <Mail className="h-4 w-4" />
                <span className="text-sm">Correo</span>
              </div>
              <p className="font-medium text-slate-900">{email}</p>
            </div>

            <div className="rounded-2xl bg-slate-50 p-4">
              <div className="mb-2 flex items-center gap-2 text-slate-500">
                <Phone className="h-4 w-4" />
                <span className="text-sm">Teléfono</span>
              </div>
              <p className="font-medium text-slate-900">{telefono}</p>
            </div>

            <div className="rounded-2xl bg-slate-50 p-4 md:col-span-2">
              <div className="mb-2 flex items-center gap-2 text-slate-500">
                <CalendarDays className="h-4 w-4" />
                <span className="text-sm">Fecha de nacimiento</span>
              </div>
              <p className="font-medium text-slate-900">{fechaNacimiento}</p>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Resumen</CardTitle>
            <CardDescription>
              Bloque listo para ampliar con historial, citas o notas.
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-3">
            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">ID</p>
              <p className="mt-1 font-medium text-slate-900">{id}</p>
            </div>

            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">Estado de ficha</p>
              <p className="mt-1 font-medium text-slate-900">Disponible</p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}