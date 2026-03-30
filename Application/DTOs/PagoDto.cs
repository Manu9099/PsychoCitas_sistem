using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Application.DTOs;

public record PagoDto(
    Guid Id,
    Guid CitaId,
    Guid PacienteId,
    decimal Monto,
    decimal MontoPagado,
    decimal Saldo,
    EstadoPago Estado,
    string? MetodoPago,
    string? NumeroOperacion,
    string? Notas,
    Guid? RegistradoPor,
    DateTime? PagadoEn,
    DateTime CreadoEn);

public record PagoResumenPacienteDto(
    Guid Id,
    Guid CitaId,
    decimal Monto,
    decimal MontoPagado,
    decimal Saldo,
    EstadoPago Estado,
    string? MetodoPago,
    DateTime? PagadoEn,
    DateTime FechaCita);