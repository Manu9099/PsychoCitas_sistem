using PsychoCitas.Application.DTOs;

namespace PsychoCitas.Application.Common.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentCheckoutDto> CrearCheckoutAsync(PaymentCheckoutRequestDto request, CancellationToken ct = default);
}