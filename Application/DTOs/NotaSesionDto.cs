using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Application.DTOs;

public record NotaSesionDto(
    Guid Id,
    Guid CitaId,
    string? ResumenSesion,
    List<string> TecnicasUsadas,
    int? EstadoAnimo,
    int? NivelAnsiedad,
    string? AvanceObjetivos,
    string? TareasAsignadas,
    string? PlanProximaSesion,
    bool EvaluacionRiesgo,
    NivelRiesgo? NivelRiesgo,
    bool Finalizada,
    DateTime ActualizadoEn
);
