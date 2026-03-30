using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Application.DTOs;

public record CitaDto(
    Guid Id,
    Guid PacienteId,
    string PacienteNombre,
    string? PacienteTelefono,
    Guid PsicologoId,
    DateTime FechaInicio,
    DateTime FechaFin,
    TipoSesion TipoSesion,
    Modalidad Modalidad,
    EstadoCita Estado,
    string? LinkVideollamada,
    int? NumeroSesion,
    bool EsPrimeraVez,
    string? NotasPrevias,
    string? MotivoCancelacion,
    EstadoPago? EstadoPago,
    decimal? MontoCita,
    decimal? MontoPagado,
    decimal? SaldoPago,
    string? MetodoPago,
    bool TieneNota);

public record CitaResumenDto(
    Guid Id,
    DateTime FechaInicio,
    DateTime FechaFin,
    string PacienteNombre,
    EstadoCita Estado,
    Modalidad Modalidad,
    TipoSesion TipoSesion,
    bool EsPrimeraVez
);
