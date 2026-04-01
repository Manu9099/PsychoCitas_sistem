import { useState } from 'react'
import { Send, Mail, MessageSquare, MessagesSquare } from 'lucide-react'
import { notificacionesApi } from '../../api/notificacionesApi'

const initialEmail = {
  email: '',
  asunto: '',
  contenido: '',
}

const initialSms = {
  telefono: '',
  mensaje: '',
}

const initialWhatsapp = {
  telefono: '',
  mensaje: '',
}

export default function NotificacionesPage() {
  const [emailForm, setEmailForm] = useState(initialEmail)
  const [smsForm, setSmsForm] = useState(initialSms)
  const [whatsappForm, setWhatsappForm] = useState(initialWhatsapp)

  const [loadingEmail, setLoadingEmail] = useState(false)
  const [loadingSms, setLoadingSms] = useState(false)
  const [loadingWhatsapp, setLoadingWhatsapp] = useState(false)

  const [feedback, setFeedback] = useState({
    type: '',
    message: '',
  })

  const setSuccess = (message) => {
    setFeedback({ type: 'success', message })
  }

  const setError = (error) => {
    const message =
      error?.response?.data?.message ||
      error?.response?.data?.title ||
      error?.message ||
      'Ocurrió un error al enviar la notificación.'
    setFeedback({ type: 'error', message })
  }

  const handleEmailSubmit = async (e) => {
    e.preventDefault()
    setLoadingEmail(true)
    setFeedback({ type: '', message: '' })

    try {
      await notificacionesApi.testEmail(emailForm)
      setSuccess('Correo de prueba enviado correctamente.')
      setEmailForm(initialEmail)
    } catch (error) {
      setError(error)
    } finally {
      setLoadingEmail(false)
    }
  }

  const handleSmsSubmit = async (e) => {
    e.preventDefault()
    setLoadingSms(true)
    setFeedback({ type: '', message: '' })

    try {
      await notificacionesApi.testSms(smsForm)
      setSuccess('SMS de prueba enviado correctamente.')
      setSmsForm(initialSms)
    } catch (error) {
      setError(error)
    } finally {
      setLoadingSms(false)
    }
  }

  const handleWhatsappSubmit = async (e) => {
    e.preventDefault()
    setLoadingWhatsapp(true)
    setFeedback({ type: '', message: '' })

    try {
      await notificacionesApi.testWhatsApp(whatsappForm)
      setSuccess('WhatsApp de prueba enviado correctamente.')
      setWhatsappForm(initialWhatsapp)
    } catch (error) {
      setError(error)
    } finally {
      setLoadingWhatsapp(false)
    }
  }

  return (
    <div className="space-y-6">
      <section className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div className="space-y-2">
          <h1 className="text-2xl font-semibold text-slate-900">Notificaciones</h1>
          <p className="text-sm text-slate-500">
            Panel para probar envíos de email, SMS y WhatsApp desde el sistema.
          </p>
        </div>

        {feedback.message ? (
          <div
            className={[
              'mt-4 rounded-2xl border px-4 py-3 text-sm',
              feedback.type === 'success'
                ? 'border-emerald-200 bg-emerald-50 text-emerald-700'
                : 'border-rose-200 bg-rose-50 text-rose-700',
            ].join(' ')}
          >
            {feedback.message}
          </div>
        ) : null}
      </section>

      <div className="grid gap-6 xl:grid-cols-3">
        <section className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="mb-5 flex items-center gap-3">
            <div className="rounded-2xl bg-violet-100 p-3 text-violet-700">
              <Mail className="h-5 w-5" />
            </div>
            <div>
              <h2 className="text-lg font-semibold text-slate-900">Email</h2>
              <p className="text-sm text-slate-500">Enviar correo de prueba</p>
            </div>
          </div>

          <form onSubmit={handleEmailSubmit} className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Correo</label>
              <input
                type="email"
                value={emailForm.email}
                onChange={(e) => setEmailForm((s) => ({ ...s, email: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="paciente@correo.com"
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Asunto</label>
              <input
                type="text"
                value={emailForm.asunto}
                onChange={(e) => setEmailForm((s) => ({ ...s, asunto: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="Confirmación de cita"
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Contenido</label>
              <textarea
                rows={5}
                value={emailForm.contenido}
                onChange={(e) => setEmailForm((s) => ({ ...s, contenido: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="Mensaje del correo..."
                required
              />
            </div>

            <button
              type="submit"
              disabled={loadingEmail}
              className="inline-flex w-full items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
            >
              <Send className="h-4 w-4" />
              {loadingEmail ? 'Enviando...' : 'Enviar email'}
            </button>
          </form>
        </section>

        <section className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="mb-5 flex items-center gap-3">
            <div className="rounded-2xl bg-amber-100 p-3 text-amber-700">
              <MessageSquare className="h-5 w-5" />
            </div>
            <div>
              <h2 className="text-lg font-semibold text-slate-900">SMS</h2>
              <p className="text-sm text-slate-500">Enviar mensaje de prueba</p>
            </div>
          </div>

          <form onSubmit={handleSmsSubmit} className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Teléfono</label>
              <input
                type="text"
                value={smsForm.telefono}
                onChange={(e) => setSmsForm((s) => ({ ...s, telefono: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="+51999999999"
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Mensaje</label>
              <textarea
                rows={6}
                value={smsForm.mensaje}
                onChange={(e) => setSmsForm((s) => ({ ...s, mensaje: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="Texto del SMS..."
                required
              />
            </div>

            <button
              type="submit"
              disabled={loadingSms}
              className="inline-flex w-full items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
            >
              <Send className="h-4 w-4" />
              {loadingSms ? 'Enviando...' : 'Enviar SMS'}
            </button>
          </form>
        </section>

        <section className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="mb-5 flex items-center gap-3">
            <div className="rounded-2xl bg-emerald-100 p-3 text-emerald-700">
              <MessagesSquare className="h-5 w-5" />
            </div>
            <div>
              <h2 className="text-lg font-semibold text-slate-900">WhatsApp</h2>
              <p className="text-sm text-slate-500">Enviar mensaje de prueba</p>
            </div>
          </div>

          <form onSubmit={handleWhatsappSubmit} className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Teléfono</label>
              <input
                type="text"
                value={whatsappForm.telefono}
                onChange={(e) => setWhatsappForm((s) => ({ ...s, telefono: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="+51999999999"
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-slate-700">Mensaje</label>
              <textarea
                rows={6}
                value={whatsappForm.mensaje}
                onChange={(e) => setWhatsappForm((s) => ({ ...s, mensaje: e.target.value }))}
                className="w-full rounded-2xl border border-slate-200 px-4 py-3 outline-none transition focus:border-violet-400"
                placeholder="Texto para WhatsApp..."
                required
              />
            </div>

            <button
              type="submit"
              disabled={loadingWhatsapp}
              className="inline-flex w-full items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
            >
              <Send className="h-4 w-4" />
              {loadingWhatsapp ? 'Enviando...' : 'Enviar WhatsApp'}
            </button>
          </form>
        </section>
      </div>
    </div>
  )
}