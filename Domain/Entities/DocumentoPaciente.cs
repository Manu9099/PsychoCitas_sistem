using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Domain.Entities;

public class DocumentoPaciente : BaseEntity
{
    public Guid PacienteId { get; private set; }
    public TipoDocumentoPaciente Tipo { get; private set; }
    public string NombreOriginal { get; private set; } = string.Empty;
    public string NombreArchivo { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string Extension { get; private set; } = string.Empty;
    public long TamanoBytes { get; private set; }
    public string UrlStorage { get; private set; } = string.Empty;
    public string? Observaciones { get; private set; }
    public bool Activo { get; private set; } = true;

    public Paciente? Paciente { get; private set; }

    protected DocumentoPaciente() { }

    public static DocumentoPaciente Crear(
        Guid pacienteId,
        TipoDocumentoPaciente tipo,
        string nombreOriginal,
        string nombreArchivo,
        string contentType,
        long tamanoBytes,
        string urlStorage,
        string? observaciones = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreOriginal);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreArchivo);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentException.ThrowIfNullOrWhiteSpace(urlStorage);

        return new DocumentoPaciente
        {
            PacienteId = pacienteId,
            Tipo = tipo,
            NombreOriginal = nombreOriginal.Trim(),
            NombreArchivo = nombreArchivo.Trim(),
            ContentType = contentType.Trim(),
            Extension = Path.GetExtension(nombreOriginal).ToLowerInvariant(),
            TamanoBytes = tamanoBytes,
            UrlStorage = urlStorage.Trim(),
            Observaciones = observaciones?.Trim()
        };
    }

    public void Desactivar() => Activo = false;
}