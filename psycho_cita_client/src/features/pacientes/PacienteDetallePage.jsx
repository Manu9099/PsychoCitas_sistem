import { useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, FileText, FolderOpen, LayoutGrid } from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Button from '../../components/ui/Button'
import EmptyState from '../../components/ui/EmptyState'
import { Card, CardContent } from '../../components/ui/Card'
import { pacientesApi } from '../../api/pacientesApi'
import PacienteResumenTab from './components/PacienteResumenTab'
import PacienteNotasTab from './components/PacienteNotasTab'
import PacienteDocumentosTab from './components/PacienteDocumentosTab'

const TABS = [
  {
    key: 'resumen',
    label: 'Resumen',
    icon: LayoutGrid,
    description: 'Ficha general, historia clínica y métricas de seguimiento.',
  },
  {
    key: 'notas',
    label: 'Notas',
    icon: FileText,
    description: 'Notas clínicas por sesión asociadas a cada cita del paciente.',
  },
  {
    key: 'documentos',
    label: 'Documentos',
    icon: FolderOpen,
    description: 'Consentimientos, fichas y archivos clínicos.',
  },
]

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

function getBadgeLabel(paciente) {
  return paciente?.activo ? 'Activo' : 'Inactivo'
}

export default function PacienteDetallePage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const [paciente, setPaciente] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [activeTab, setActiveTab] = useState('resumen')

  useEffect(() => {
    let ignore = false

    const loadPaciente = async () => {
      setLoading(true)
      setError('')

      try {
        const data = await pacientesApi.getById(id)
        if (!ignore) {
          setPaciente(data)
        }
      } catch (err) {
        if (!ignore) {
          setPaciente(null)
          setError(
            err?.response?.data?.message ||
              err?.response?.data?.title ||
              'No se pudo cargar el paciente.',
          )
        }
      } finally {
        if (!ignore) {
          setLoading(false)
        }
      }
    }

    loadPaciente()

    return () => {
      ignore = true
    }
  }, [id])

  const title = useMemo(() => getNombre(paciente), [paciente])
  const documento = useMemo(() => getDocumento(paciente), [paciente])
  const email = useMemo(() => getEmail(paciente), [paciente])
  const telefono = useMemo(() => getTelefono(paciente), [paciente])

  if (loading) {
    return (
      <Card>
        <CardContent>
          <p className="text-sm text-slate-500">Cargando paciente...</p>
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <EmptyState
        title="No se pudo cargar el paciente"
        description={error}
        actionLabel="Volver a pacientes"
        onAction={() => navigate('/app/pacientes')}
      />
    )
  }

  if (!paciente) {
    return (
      <EmptyState
        title="Paciente no encontrado"
        description="No se encontró información para este registro."
        actionLabel="Volver a pacientes"
        onAction={() => navigate('/app/pacientes')}
      />
    )
  }

  return (
    <div className="space-y-6">
      <PageHeader
        eyebrow="Expediente clínico"
        title={title}
        description="Centraliza la revisión general, notas por sesión y documentos del paciente desde una sola vista."
        actions={
          <Button variant="secondary" onClick={() => navigate('/app/pacientes')}>
            <ArrowLeft className="h-4 w-4" />
            Volver
          </Button>
        }
      />

      <Card>
        <CardContent className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            <div>
              <p className="text-sm text-slate-500">Documento</p>
              <p className="mt-1 font-medium text-slate-900">{documento}</p>
            </div>
            <div>
              <p className="text-sm text-slate-500">Correo</p>
              <p className="mt-1 font-medium text-slate-900">{email}</p>
            </div>
            <div>
              <p className="text-sm text-slate-500">Teléfono</p>
              <p className="mt-1 font-medium text-slate-900">{telefono}</p>
            </div>
            <div>
              <p className="text-sm text-slate-500">Estado</p>
              <p className="mt-1">
                <span className="rounded-full bg-emerald-50 px-3 py-1 text-sm font-medium text-emerald-700">
                  {getBadgeLabel(paciente)}
                </span>
              </p>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="rounded-3xl border border-slate-200 bg-white p-2 shadow-sm">
        <div className="grid gap-2 md:grid-cols-3">
          {TABS.map((tab) => {
            const Icon = tab.icon
            const isActive = activeTab === tab.key

            return (
              <button
                key={tab.key}
                type="button"
                onClick={() => setActiveTab(tab.key)}
                className={[
                  'rounded-2xl px-4 py-4 text-left transition',
                  isActive
                    ? 'bg-slate-900 text-white'
                    : 'bg-white text-slate-700 hover:bg-slate-50',
                ].join(' ')}
              >
                <div className="flex items-start gap-3">
                  <div
                    className={[
                      'mt-0.5 flex h-10 w-10 items-center justify-center rounded-2xl',
                      isActive ? 'bg-white/10' : 'bg-slate-100',
                    ].join(' ')}
                  >
                    <Icon className="h-5 w-5" />
                  </div>

                  <div>
                    <p className="font-medium">{tab.label}</p>
                    <p
                      className={[
                        'mt-1 text-sm leading-6',
                        isActive ? 'text-slate-200' : 'text-slate-500',
                      ].join(' ')}
                    >
                      {tab.description}
                    </p>
                  </div>
                </div>
              </button>
            )
          })}
        </div>
      </div>

      {activeTab === 'resumen' && <PacienteResumenTab paciente={paciente} />}
      {activeTab === 'notas' && <PacienteNotasTab pacienteId={id} />}
      {activeTab === 'documentos' && <PacienteDocumentosTab pacienteId={id} />}
    </div>
  )
}
