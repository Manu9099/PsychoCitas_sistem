import { api } from './axios'

export const pagosApi = {
  registrar: async (payload) => {
    const { data } = await api.post('/api/pagos/registrar', payload)
    return data
  },

  exonerar: async (payload) => {
    const { data } = await api.post('/api/pagos/exonerar', payload)
    return data
  },

  getByCita: async (citaId) => {
    const { data } = await api.get(`/api/pagos/cita/${citaId}`)
    return data
  },

  getByPaciente: async (pacienteId) => {
    const { data } = await api.get(`/api/pagos/paciente/${pacienteId}`)
    return data
  },
}