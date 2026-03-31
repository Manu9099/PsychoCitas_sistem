import { api } from './axios'

export const citasApi = {
  getAll: async () => {
    const { data } = await api.get('/api/citas')
    return data
  },

  getAgendaHoy: async (psicologoId) => {
    const { data } = await api.get('/api/citas/agenda/hoy', {
      params: psicologoId ? { psicologoId } : {},
    })
    return data
  },

  getById: async (id) => {
    const { data } = await api.get(`/api/citas/${id}`)
    return data
  },

  cancelar: async (id, motivo) => {
    await api.patch(`/api/citas/${id}/cancelar`, { motivo })
  },

  completar: async (id) => {
    await api.patch(`/api/citas/${id}/completar`)
  },

  noAsistio: async (id) => {
    await api.patch(`/api/citas/${id}/no-asistio`)
  },
}
