import { api } from './axios'

export async function loginRequest(payload) {
  const { data } = await api.post('/api/auth/login', payload)
  return data
}