import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../app/authStore'

export function AuthGuard({ children }) {
  const token = useAuthStore((s) => s.token)
  if (!token) return <Navigate to="/login" replace />
  return children
}