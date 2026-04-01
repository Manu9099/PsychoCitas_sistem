import { api } from './axios'

export const notasApi = {
  getByCita: async (citaId) => {
    const { data } = await api.get(`/api/citas/${citaId}/nota`)
    return data
  },

  guardar: async (citaId, payload) => {
    const body = {
      resumenSesion: payload?.resumenSesion?.trim() || null,
      tecnicasUsadas: Array.isArray(payload?.tecnicasUsadas)
        ? payload.tecnicasUsadas
        : [],
      estadoAnimo: payload?.estadoAnimo ?? null,
      nivelAnsiedad: payload?.nivelAnsiedad ?? null,
      avanceObjetivos: payload?.avanceObjetivos?.trim() || null,
      tareasAsignadas: payload?.tareasAsignadas?.trim() || null,
      observaciones: null,
      planProximaSesion: payload?.planProximaSesion?.trim() || null,
      evaluacionRiesgo: Boolean(payload?.evaluacionRiesgo),
      nivelRiesgo: payload?.nivelRiesgo || null,
      accionesRiesgo: null,
      finalizar: Boolean(payload?.finalizar),
    }

    const { data } = await api.put(`/api/citas/${citaId}/nota`, body)
    return data
  },

  finalizar: async (citaId) => {
    const { data } = await api.patch(`/api/citas/${citaId}/nota/finalizar`)
    return data
  },
}
