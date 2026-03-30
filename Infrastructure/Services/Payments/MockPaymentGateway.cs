using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;

namespace PsychoCitas.Infrastructure.Services.Payments;

public class MockPaymentGateway : IPaymentGateway
{
    public Task<PaymentCheckoutDto> CrearCheckoutAsync(PaymentCheckoutRequestDto request, CancellationToken ct = default)
    {
        var externalReference = $"mock-{Guid.NewGuid():N}";
        var checkoutUrl = $"https://mock-payments.local/checkout/{externalReference}";
        var expiraEn = DateTime.UtcNow.AddHours(2);

        return Task.FromResult(new PaymentCheckoutDto(
            externalReference,
            checkoutUrl,
            expiraEn));
    }
}