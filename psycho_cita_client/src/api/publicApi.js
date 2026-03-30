import { api } from './axios'

export const publicApi = {
  getDisponibilidad: async ({ psicologoId, fecha, duracionMinutos = 50 }) => {
    const { data } = await api.get('/api/public/disponibilidad', {
      params: { psicologoId, fecha, duracionMinutos },
    })
    return data
  },

  crearCita: async (payload) => {
    const { data } = await api.post('/api/public/citas', payload)
    return data
  },

  crearCheckout: async (payload) => {
    const { data } = await api.post('/api/public/checkout', payload)
    return data
  },

  confirmarMock: async (payload) => {
    const { data } = await api.post('/api/public/callback/mock', payload)
    return data
  },
}