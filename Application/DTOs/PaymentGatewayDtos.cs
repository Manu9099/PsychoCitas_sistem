namespace PsychoCitas.Application.DTOs;

public record PaymentCheckoutRequestDto(
    Guid PagoId,
    Guid CitaId,
    Guid PacienteId,
    decimal Monto,
    string Moneda,
    string Descripcion,
    string EmailPaciente);

public record PaymentCheckoutDto(
    string ExternalReference,
    string CheckoutUrl,
    DateTime? ExpiraEn);