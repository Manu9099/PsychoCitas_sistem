namespace PsychoCitas.Application.DTOs;

public record PublicSlotDto(DateTime Inicio, DateTime Fin, bool Disponible);

public record PublicDisponibilidadDto(
    Guid PsicologoId,
    DateOnly Fecha,
    int DuracionMinutos,
    IReadOnlyCollection<PublicSlotDto> Slots);

public record PublicCitaCreadaDto(
    Guid CitaId,
    Guid PacienteId,
    string Mensaje,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? EmailDestino);