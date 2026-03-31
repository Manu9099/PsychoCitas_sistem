import { api } from './axios'

/**
 * API client para endpoints del dashboard
 */
export const dashboardApi = {
  /**
   * GET /api/dashboard/resumen
   * Obtiene resumen con métricas del dashboard:
   * - citasHoy: número de citas programadas para hoy
   * - pacientesTotales: número de pacientes activos
   * - totalPagado: suma total de pagos realizados (Pagado + Parcial)
   *
   * @returns {Promise<{ citasHoy: number, pacientesTotales: number, totalPagado: number }>}
   */
  getResumen: async () => {
    const { data } = await api.get('/api/dashboard/resumen')
    return data
  },
}