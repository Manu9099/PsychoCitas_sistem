import { api } from './axios'

export const documentosApi = {
  listar: async (pacienteId) => {
    const { data } = await api.get(`/api/pacientes/${pacienteId}/documentos`)
    return data
  },

  subir: async (pacienteId, payload) => {
    const formData = new FormData()
    formData.append('archivo', payload.archivo)
    formData.append('tipo', String(payload.tipo))
    formData.append('observaciones', payload.observaciones || '')

    const { data } = await api.post(
      `/api/pacientes/${pacienteId}/documentos`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      },
    )

    return data
  },

  descargar: async (pacienteId, documentoId) => {
    const response = await api.get(
      `/api/pacientes/${pacienteId}/documentos/${documentoId}/download`,
      {
        responseType: 'blob',
      },
    )

    return {
      blob: response.data,
      contentType: response.headers['content-type'],
      fileName:
        getFileNameFromDisposition(response.headers['content-disposition']) ||
        'documento',
    }
  },
}

function getFileNameFromDisposition(contentDisposition) {
  if (!contentDisposition) return ''

  const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
  if (utf8Match?.[1]) return decodeURIComponent(utf8Match[1])

  const asciiMatch = contentDisposition.match(/filename="?([^"]+)"?/i)
  return asciiMatch?.[1] || ''
}
