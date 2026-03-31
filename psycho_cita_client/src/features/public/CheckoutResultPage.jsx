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

function getCheckoutUrl(checkout) {
  return (
    checkout?.checkoutUrl ||
    checkout?.CheckoutUrl ||
    checkout?.url ||
    checkout?.paymentUrl ||
    checkout?.redirectUrl ||
    ''
  )
}

function getExternalReference(checkout) {
  return (
    checkout?.externalReference ||
    checkout?.ExternalReference ||
    ''
  )
}

function getMonto(checkout) {
  return Number(
    checkout?.monto ??
    checkout?.Monto ??
    checkout?.amount ??
    0
  )
}

export default function CheckoutResultPage() {
  const location = useLocation()
  const navigate = useNavigate()

  const booking = location.state?.booking || null
  const checkout = location.state?.checkout || null

  const checkoutUrl = getCheckoutUrl(checkout)
  const externalReference = getExternalReference(checkout)
  const monto = getMonto(checkout)

  const handleOpenCheckout = () => {
    console.log('checkout state', { booking, checkout, checkoutUrl, externalReference, monto })

    if (!checkoutUrl) {
      alert('No llegó checkoutUrl desde el backend')
      return
    }

    window.open(checkoutUrl, '_blank', 'noopener,noreferrer')
  }

  const handleMockConfirm = async () => {
    try {
      console.log('mock input', { booking, checkout, externalReference, monto })

      if (!externalReference) {
        alert('No llegó externalReference desde el backend')
        return
      }

      if (!monto || monto <= 0) {
        alert('No llegó un monto válido desde el backend')
        return
      }

      const payload = {
        externalReference,
        montoPagado: monto,
        metodoPago: 'MockCheckout',
        numeroOperacion: `MOCK-${Date.now()}`,
        payloadRaw: JSON.stringify({
          source: 'frontend-mock',
          booking,
          checkout,
        }),
      }

      console.log('mock confirm payload', payload)

      const result = await publicApi.confirmarMock(payload)
      console.log('mock confirm result', result)

      alert('Pago mock confirmado')
      navigate('/app/pagos')
    } catch (err) {
      console.error('mock confirm error', err?.response?.data || err)
      alert(
        err?.response?.data?.message ||
        err?.response?.data?.title ||
        'No se pudo confirmar el pago mock'
      )
    }
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
              <p className="text-sm text-slate-500">Referencia externa</p>
              <p className="mt-1 break-all font-medium text-slate-900">
                {externalReference || 'No disponible'}
              </p>
            </div>

            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">Checkout URL</p>
              <p className="mt-1 break-all font-medium text-slate-900">
                {checkoutUrl || 'No disponible'}
              </p>
            </div>

            <div className="rounded-2xl border border-slate-200 p-4">
              <p className="text-sm text-slate-500">Monto</p>
              <p className="mt-1 font-medium text-slate-900">
                {monto || 'No disponible'}
              </p>
            </div>

            <div className="flex flex-col gap-3 md:flex-row">
              <Button onClick={handleOpenCheckout}>
                <ExternalLink className="h-4 w-4" />
                Ir al checkout
              </Button>

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