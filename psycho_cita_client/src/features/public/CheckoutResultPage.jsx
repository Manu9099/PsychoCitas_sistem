import { useLocation, useNavigate } from 'react-router-dom'
import { CheckCircle2, ExternalLink, CreditCard } from 'lucide-react'
import PageHeader from '../../components/ui/PageHeader'
import Button from '../../components/ui/Button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '../../components/ui/Card'
import { publicApi } from '../../api/publicApi'

export default function CheckoutResultPage() {
  const location = useLocation()
  const navigate = useNavigate()

  const booking = location.state?.booking
  const checkout = location.state?.checkout
  const checkoutUrl = location.state?.checkoutUrl || ''

  const handleMockConfirm = async () => {
    const citaId = booking?.citaId || booking?.id
    const pagoIntentId =
      checkout?.pagoIntentId || checkout?.intentId || checkout?.id

    if (!citaId || !pagoIntentId) return

    await publicApi.confirmarMock({
      citaId,
      pagoIntentId,
      estado: 'Confirmado',
    })

    navigate('/app/pagos')
  }

  return (
    <div className="min-h-screen bg-slate-50 px-4 py-8 md:px-6">
      <div className="mx-auto max-w-3xl space-y-6">
        <PageHeader
          eyebrow="Reserva pública"
          title="Checkout generado"
          description="Tu cita fue creada. Continúa con el pago o simúlalo en desarrollo."
        />

        <Card>
          <CardHeader>
            <CardTitle>Resumen</CardTitle>
            <CardDescription>Estado actual del flujo.</CardDescription>
          </CardHeader>

          <CardContent className="space-y-4">
            <div className="flex items-center gap-3 rounded-2xl bg-emerald-50 p-4 text-emerald-700">
              <CheckCircle2 className="h-5 w-5" />
              <p className="font-medium">Cita creada correctamente</p>
            </div>

            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">Cita ID</p>
              <p className="mt-1 break-all font-medium text-slate-900">
                {booking?.citaId || booking?.id || 'No disponible'}
              </p>
            </div>

            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">Intento de pago</p>
              <p className="mt-1 break-all font-medium text-slate-900">
                {checkout?.pagoIntentId || checkout?.intentId || checkout?.id || 'No disponible'}
              </p>
            </div>

            <div className="flex flex-col gap-3 md:flex-row">
              {checkoutUrl ? (
                <a
                  href={checkoutUrl}
                  target="_blank"
                  rel="noreferrer"
                  className="inline-flex items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white"
                >
                  Ir al checkout
                  <ExternalLink className="h-4 w-4" />
                </a>
              ) : (
                <div className="rounded-2xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-700">
                  No vino URL de checkout. Revisa la respuesta real del backend.
                </div>
              )}

              <Button variant="secondary" onClick={handleMockConfirm}>
                <CreditCard className="h-4 w-4" />
                Confirmar mock
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}