using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Application.DTOs;

public record DocumentoPacienteDto(
    Guid Id,
    Guid PacienteId,
    TipoDocumentoPaciente Tipo,
    string NombreOriginal,
    string ContentType,
    string Extension,
    long TamanoBytes,
    DateTime CreadoEn,
    string? Observaciones);

public record DocumentoDescargaDto(
    string NombreOriginal,
    string ContentType,
    Stream Contenido);