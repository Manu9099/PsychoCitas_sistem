import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { loginRequest, normalizeLoginResponse } from '../../api/authApi'
import { useAuthStore } from '../../app/authStore'

export default function LoginPage() {
  const navigate = useNavigate()
  const saveLogin = useAuthStore((s) => s.login)

  const [form, setForm] = useState({
    email: '',
    password: '',
  })

  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')

    try {
      const raw = await loginRequest(form)
      const auth = normalizeLoginResponse(raw)

      if (!auth.token) {
        throw new Error('La respuesta no devolvió token')
      }

      saveLogin(auth)
      navigate('/app')
    } catch (err) {
      setError(
        err?.response?.data?.message ||
        err?.response?.data?.title ||
        err?.message ||
        'No se pudo iniciar sesión'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50 p-6">
      <div className="w-full max-w-md rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
        <p className="mb-2 text-sm font-semibold text-violet-600">PsychoCitas</p>
        <h1 className="text-2xl font-semibold tracking-tight">Iniciar sesión</h1>
        <p className="mt-2 text-sm text-slate-500">
          Accede al panel interno de citas y pacientes.
        </p>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <input
            name="email"
            type="email"
            placeholder="Correo"
            value={form.email}
            onChange={handleChange}
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-violet-500"
          />

          <input
            name="password"
            type="password"
            placeholder="Contraseña"
            value={form.password}
            onChange={handleChange}
            className="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-violet-500"
          />

          {error ? (
            <p className="text-sm text-rose-600">{error}</p>
          ) : null}

          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white transition hover:opacity-90 disabled:opacity-60"
          >
            {loading ? 'Ingresando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  )
}