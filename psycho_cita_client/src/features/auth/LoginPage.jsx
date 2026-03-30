import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { loginRequest } from '../../api/authApi'
import { useAuthStore } from '../../app/authStore'

export default function LoginPage() {
  const navigate = useNavigate()
  const saveLogin = useAuthStore((s) => s.login)

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')

    try {
      const data = await loginRequest({ email, password })
      saveLogin({
        token: data.token,
        user: {
          nombre: data.nombre,
          email: data.email,
          roles: data.roles,
        },
      })
      navigate('/app')
    } catch (err) {
      setError(err?.response?.data?.message || 'No se pudo iniciar sesión')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-6">
      <div className="w-full max-w-md rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
        <p className="mb-2 text-sm font-semibold text-violet-600">PsychoCitas</p>
        <h1 className="text-2xl font-semibold">Iniciar sesión</h1>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3"
            type="email"
            placeholder="Correo"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <input
            className="w-full rounded-2xl border border-slate-300 px-4 py-3"
            type="password"
            placeholder="Contraseña"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          {error ? <p className="text-sm text-rose-600">{error}</p> : null}

          <button
            disabled={loading}
            className="w-full rounded-2xl bg-slate-900 px-4 py-3 text-white"
          >
            {loading ? 'Ingresando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  )
}