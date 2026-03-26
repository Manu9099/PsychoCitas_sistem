namespace PsychoCitas.Application.DTOs;

public record PacienteDto(
    Guid Id,
    string Nombres,
    string Apellidos,
    string NombreCompleto,
    int? Edad,
    string? Dni,
    string? Email,
    string? Telefono,
    string? Genero,
    string? Ocupacion,
    string? ReferidoPor,
    bool Activo,
    DateTime CreadoEn
);

public record PacienteDetalleDto(
    Guid Id,
    string Nombres,
    string Apellidos,
    string NombreCompleto,
    int? Edad,
    string? Dni,
    string? Email,
    string? Telefono,
    string? TelefonoEmergencia,
    string? ContactoEmergencia,
    string? Genero,
    string? Ocupacion,
    string? EstadoCivil,
    string? Direccion,
    string? ReferidoPor,
    bool Activo,
    HistoriaClinicaDto? Historia,
    int SesionesCompletadas,
    int Inasistencias,
    DateTime? UltimaSesion,
    decimal DeudaPendiente
);

public record HistoriaClinicaDto(
    Guid Id,
    string? MotivoConsulta,
    string? DiagnosticoInicial,
    string? DiagnosticoCie11,
    string? ObjetivosTerapeuticos,
    string? MedicacionActual,
    DateOnly FechaIngreso,
    bool EstaActivo
);
