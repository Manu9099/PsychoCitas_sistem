import { CalendarDays, DollarSign, FileText, HeartPulse, PhoneCall } from 'lucide-react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../../components/ui/Card'

function formatCurrency(value) {
  if (typeof value !== 'number') return 'S/ 0.00'

  return new Intl.NumberFormat('es-PE', {
    style: 'currency',
    currency: 'PEN',
    minimumFractionDigits: 2,
  }).format(value)
}

function formatDate(value) {
  if (!value) return 'No registrada'

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleDateString('es-PE', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

function SummaryStat({ icon: Icon, label, value }) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
      <div className="mb-3 flex h-10 w-10 items-center justify-center rounded-2xl bg-white shadow-sm">
        <Icon className="h-5 w-5 text-slate-700" />
      </div>
      <p className="text-sm text-slate-500">{label}</p>
      <p className="mt-1 text-xl font-semibold tracking-tight text-slate-900">{value}</p>
    </div>
  )
}

function DetailItem({ label, value }) {
  return (
    <div className="rounded-2xl border border-slate-200 p-4">
      <p className="text-sm text-slate-500">{label}</p>
      <p className="mt-1 text-sm font-medium text-slate-900">{value || 'No registrado'}</p>
    </div>
  )
}

export default function PacienteResumenTab({ paciente }) {
  const historia = paciente?.historia

  return (
    <div className="space-y-6">
      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <SummaryStat
          icon={HeartPulse}
          label="Sesiones completadas"
          value={paciente?.sesionesCompletadas ?? 0}
        />
        <SummaryStat
          icon={CalendarDays}
          label="Inasistencias"
          value={paciente?.inasistencias ?? 0}
        />
        <SummaryStat
          icon={FileText}
          label="Última sesión"
          value={formatDate(paciente?.ultimaSesion)}
        />
        <SummaryStat
          icon={DollarSign}
          label="Deuda pendiente"
          value={formatCurrency(paciente?.deudaPendiente)}
        />
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
        <Card>
          <CardHeader>
            <CardTitle>Datos del paciente</CardTitle>
            <CardDescription>Información principal y datos de contacto.</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-2">
            <DetailItem label="Nombres" value={paciente?.nombres} />
            <DetailItem label="Apellidos" value={paciente?.apellidos} />
            <DetailItem label="Nombre completo" value={paciente?.nombreCompleto} />
            <DetailItem label="Documento" value={paciente?.dni} />
            <DetailItem label="Correo" value={paciente?.email} />
            <DetailItem label="Teléfono" value={paciente?.telefono} />
            <DetailItem label="Género" value={paciente?.genero} />
            <DetailItem label="Ocupación" value={paciente?.ocupacion} />
            <DetailItem label="Estado civil" value={paciente?.estadoCivil} />
            <DetailItem label="Dirección" value={paciente?.direccion} />
            <DetailItem label="Referido por" value={paciente?.referidoPor} />
            <DetailItem label="Edad" value={paciente?.edad ? `${paciente.edad} años` : 'No registrada'} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Contacto de emergencia</CardTitle>
            <CardDescription>Referencia para seguimiento y soporte.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailItem label="Contacto" value={paciente?.contactoEmergencia} />
            <DetailItem label="Teléfono" value={paciente?.telefonoEmergencia} />
            <div className="rounded-2xl border border-dashed border-slate-300 p-4">
              <div className="flex items-start gap-3">
                <PhoneCall className="mt-0.5 h-5 w-5 text-slate-500" />
                <div>
                  <p className="text-sm font-medium text-slate-900">Ficha activa</p>
                  <p className="mt-1 text-sm text-slate-500">
                    {paciente?.activo
                      ? 'El paciente figura como activo dentro del sistema.'
                      : 'El paciente figura como inactivo dentro del sistema.'}
                  </p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>Historia clínica</CardTitle>
          <CardDescription>Resumen base para el seguimiento terapéutico.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <DetailItem label="Motivo de consulta" value={historia?.motivoConsulta} />
          <DetailItem label="Diagnóstico inicial" value={historia?.diagnosticoInicial} />
          <DetailItem label="Diagnóstico CIE-11" value={historia?.diagnosticoCie11} />
          <DetailItem label="Medicación actual" value={historia?.medicacionActual} />
          <DetailItem label="Fecha de ingreso" value={historia?.fechaIngreso || 'No registrada'} />
          <DetailItem label="Estado de historia" value={historia?.estaActivo ? 'Activa' : 'Inactiva'} />
          <div className="md:col-span-2">
            <DetailItem label="Objetivos terapéuticos" value={historia?.objetivosTerapeuticos} />
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
