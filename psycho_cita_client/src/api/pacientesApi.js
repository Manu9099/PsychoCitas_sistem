import { api } from './axios'

export const pacientesApi = {
  buscar: async (termino) => {
    const { data } = await api.get('/api/pacientes/buscar', {
      params: { termino },
    })
    return data
  },

  getById: async (id) => {
    const { data } = await api.get(`/api/pacientes/${id}`)
    return data
  },

  crear: async (payload) => {
    const { data } = await api.post('/api/pacientes', payload)
    return data
  },
}