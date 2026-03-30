import { api } from './axios'

export async function loginRequest(payload) {
  const { data } = await api.post('/api/auth/login', payload)
  return data
}

export function normalizeLoginResponse(data) {
  return {
    token: data?.token || data?.accessToken || data?.jwt || '',
    user: {
      id: data?.userId || data?.id || null,
      nombre: data?.nombre || data?.name || data?.fullName || '',
      email: data?.email || '',
      roles: data?.roles || data?.role ? [data.role].flat() : [],
    },
  }
}