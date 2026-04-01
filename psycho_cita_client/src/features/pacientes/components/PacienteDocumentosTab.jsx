import { useEffect, useMemo, useState } from 'react'
import { AlertCircle, CheckCircle2, Download, FileUp, Files, UploadCloud } from 'lucide-react'
import Button from '../../../components/ui/Button'
import EmptyState from '../../../components/ui/EmptyState'
import Input from '../../../components/ui/Input'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../../components/ui/Card'
import { documentosApi } from '../../../api/documentosApi'

const DOCUMENT_TYPE_OPTIONS = [
  { value: 0, label: 'Consentimiento' },
  { value: 1, label: 'Ficha' },
  { value: 2, label: 'Archivo clínico' },
]

function formatFileSize(bytes) {
  const value = Number(bytes)
  if (!Number.isFinite(value) || value <= 0) return '0 B'

  const units = ['B', 'KB', 'MB', 'GB']
  const power = Math.min(Math.floor(Math.log(value) / Math.log(1024)), units.length - 1)
  const size = value / 1024 ** power

  return `${size.toFixed(size >= 10 || power === 0 ? 0 : 1)} ${units[power]}`
}

function formatDate(value) {
  if (!value) return 'Sin fecha'

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleString('es-PE', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function normalizeDocuments(data) {
  return Array.isArray(data) ? data : []
}

function triggerBrowserDownload(blob, fileName) {
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement('a')
  anchor.href = url
  anchor.download = fileName || 'documento'
  document.body.appendChild(anchor)
  anchor.click()
  anchor.remove()
  URL.revokeObjectURL(url)
}

export default function PacienteDocumentosTab({ pacienteId }) {
  const [documentos, setDocumentos] = useState([])
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)
  const [downloadingId, setDownloadingId] = useState('')
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [form, setForm] = useState({
    tipo: 0,
    observaciones: '',
    archivo: null,
  })

  const selectedFileName = useMemo(() => form.archivo?.name || '', [form.archivo])

  const loadDocumentos = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await documentosApi.listar(pacienteId)
      const normalized = normalizeDocuments(data).sort(
        (a, b) => new Date(b?.creadoEn || 0) - new Date(a?.creadoEn || 0),
      )
      setDocumentos(normalized)
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudieron cargar los documentos.',
      )
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadDocumentos()
  }, [pacienteId])

  const handleFileChange = (event) => {
    const file = event?.target?.files?.[0] || null
    setForm((prev) => ({ ...prev, archivo: file }))
    setError('')
    setSuccess('')
  }

  const handleUpload = async () => {
    if (!form.archivo) {
      setError('Selecciona un archivo antes de subirlo.')
      return
    }

    setUploading(true)
    setError('')
    setSuccess('')

    try {
      await documentosApi.subir(pacienteId, form)
      setSuccess('Documento subido correctamente.')
      setForm({
        tipo: 0,
        observaciones: '',
        archivo: null,
      })
      await loadDocumentos()
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo subir el documento.',
      )
    } finally {
      setUploading(false)
    }
  }

  const handleDownload = async (documento) => {
    setDownloadingId(documento.id)
    setError('')
    setSuccess('')

    try {
      const result = await documentosApi.descargar(pacienteId, documento.id)
      triggerBrowserDownload(result.blob, result.fileName || documento.nombreOriginal)
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.response?.data?.title ||
          'No se pudo descargar el documento.',
      )
    } finally {
      setDownloadingId('')
    }
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Subir documento</CardTitle>
          <CardDescription>
            Adjunta consentimientos, fichas o archivos clínicos al expediente del paciente.
          </CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 lg:grid-cols-[0.9fr_1.1fr]">
          <div className="space-y-4">
            {(error || success) && (
              <div
                className={[
                  'rounded-2xl border px-4 py-3 text-sm',
                  error
                    ? 'border-rose-200 bg-rose-50 text-rose-700'
                    : 'border-emerald-200 bg-emerald-50 text-emerald-700',
                ].join(' ')}
              >
                <div className="flex items-start gap-2">
                  {error ? (
                    <AlertCircle className="mt-0.5 h-4 w-4 shrink-0" />
                  ) : (
                    <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0" />
                  )}
                  <span>{error || success}</span>
                </div>
              </div>
            )}

            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">
                Tipo de documento
              </label>
              <select
                value={form.tipo}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, tipo: Number(event.target.value) }))
                }
                className="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 outline-none transition focus:border-violet-500"
              >
                {DOCUMENT_TYPE_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>

            <Input
              label="Observaciones"
              value={form.observaciones}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, observaciones: event.target.value }))
              }
              placeholder="Ej.: consentimiento firmado en primera consulta"
            />

            <div className="rounded-2xl border border-dashed border-slate-300 bg-slate-50 p-5">
              <label
                htmlFor="document-upload"
                className="flex cursor-pointer flex-col items-center justify-center gap-3 text-center"
              >
                <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-white shadow-sm">
                  <UploadCloud className="h-6 w-6 text-slate-700" />
                </div>
                <div>
                  <p className="font-medium text-slate-900">Seleccionar archivo</p>
                  <p className="mt-1 text-sm text-slate-500">
                    Tamaño máximo recomendado: 10 MB.
                  </p>
                </div>
                <input
                  id="document-upload"
                  type="file"
                  className="hidden"
                  onChange={handleFileChange}
                />
              </label>

              {selectedFileName && (
                <div className="mt-4 rounded-2xl border border-slate-200 bg-white px-4 py-3 text-sm text-slate-700">
                  Archivo seleccionado: <span className="font-medium">{selectedFileName}</span>
                </div>
              )}
            </div>

            <Button onClick={handleUpload} loading={uploading}>
              <FileUp className="h-4 w-4" />
              Subir documento
            </Button>
          </div>

          <div className="rounded-3xl border border-slate-200 bg-slate-50 p-5">
            <div className="flex items-start gap-3">
              <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-white shadow-sm">
                <Files className="h-5 w-5 text-slate-700" />
              </div>
              <div>
                <p className="font-medium text-slate-900">Buenas prácticas</p>
                <ul className="mt-2 space-y-2 text-sm text-slate-500">
                  <li>• Usa nombres claros si el archivo fue preparado fuera del sistema.</li>
                  <li>• Adjunta observaciones breves si el documento requiere contexto.</li>
                  <li>• Verifica que el archivo corresponda al paciente correcto antes de subirlo.</li>
                </ul>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Documentos del paciente</CardTitle>
          <CardDescription>Repositorio interno de archivos asociados al expediente.</CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <p className="text-sm text-slate-500">Cargando documentos...</p>
          ) : documentos.length === 0 ? (
            <EmptyState
              title="Sin documentos cargados"
              description="Todavía no se han adjuntado archivos para este paciente."
            />
          ) : (
            <div className="space-y-3">
              {documentos.map((documento) => (
                <div
                  key={documento.id}
                  className="flex flex-col gap-4 rounded-2xl border border-slate-200 p-4 lg:flex-row lg:items-center lg:justify-between"
                >
                  <div className="min-w-0">
                    <p className="truncate font-medium text-slate-900">
                      {documento.nombreOriginal}
                    </p>
                    <p className="mt-1 text-sm text-slate-500">
                      {documento.tipo} · {formatFileSize(documento.tamanoBytes)} ·{' '}
                      {formatDate(documento.creadoEn)}
                    </p>
                    {documento.observaciones && (
                      <p className="mt-2 text-sm text-slate-600">{documento.observaciones}</p>
                    )}
                  </div>

                  <Button
                    variant="secondary"
                    onClick={() => handleDownload(documento)}
                    loading={downloadingId === documento.id}
                  >
                    <Download className="h-4 w-4" />
                    Descargar
                  </Button>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
