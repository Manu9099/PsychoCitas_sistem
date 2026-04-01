import { api } from './axios'

export const notificacionesApi = {
  testEmail: async (payload) => {
    const { data } = await api.post('/api/notificaciones/test/email', payload)
    return data
  },

  testSms: async (payload) => {
    const { data } = await api.post('/api/notificaciones/test/sms', payload)
    return data
  },

  testWhatsApp: async (payload) => {
    const { data } = await api.post('/api/notificaciones/test/whatsapp', payload)
    return data
  },
}