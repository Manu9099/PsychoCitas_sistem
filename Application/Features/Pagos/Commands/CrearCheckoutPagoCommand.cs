using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Commands;

public record CrearCheckoutPagoCommand(
    Guid CitaId,
    decimal MontoTotal,
    string Moneda = "PEN"
) : IRequest<CrearCheckoutPagoResponse>;

public record CrearCheckoutPagoResponse(
    Guid PagoId,
    Guid IntentoPagoId,
    string CheckoutUrl,
    string ExternalReference,
    DateTime? ExpiraEn,
    decimal Monto,
    string Moneda);

public class CrearCheckoutPagoValidator : AbstractValidator<CrearCheckoutPagoCommand>
{
    public CrearCheckoutPagoValidator()
    {
        RuleFor(x => x.CitaId).NotEmpty();
        RuleFor(x => x.MontoTotal).GreaterThan(0);
        RuleFor(x => x.Moneda).NotEmpty().MaximumLength(10);
    }
}

public class CrearCheckoutPagoHandler(
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IPaymentGateway paymentGateway)
    : IRequestHandler<CrearCheckoutPagoCommand, CrearCheckoutPagoResponse>
{
    public async Task<CrearCheckoutPagoResponse> Handle(CrearCheckoutPagoCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);

        var paciente = await uow.Pacientes.GetByIdAsync(cita.PacienteId, ct)
            ?? throw new NotFoundException(nameof(Paciente), cita.PacienteId);

        if (string.IsNullOrWhiteSpace(paciente.Email))
            throw new DomainException("El paciente necesita email para checkout automático.");

        var pago = await uow.Pagos.GetByCitaIdAsync(cmd.CitaId, ct);

        if (pago is null)
        {
            pago = Pago.Crear(
                cmd.CitaId,
                cita.PacienteId,
                cmd.MontoTotal,
                currentUser.IsAuthenticated ? currentUser.UserId : null);

            await uow.Pagos.AddAsync(pago, ct);
            await uow.SaveChangesAsync(ct);
        }

        var checkout = await paymentGateway.CrearCheckoutAsync(
            new PsychoCitas.Application.DTOs.PaymentCheckoutRequestDto(
                pago.Id,
                cita.Id,
                paciente.Id,
                cmd.MontoTotal,
                cmd.Moneda,
                $"Pago cita {cita.FechaInicio:dd/MM/yyyy HH:mm}",
                paciente.Email),
            ct);

        var intento = IntentoPago.Crear(
            pago.Id,
            ProveedorPago.MockCheckout,
            cmd.MontoTotal,
            cmd.Moneda,
            checkout.ExternalReference,
            checkout.CheckoutUrl,
            checkout.ExpiraEn);

        intento.MarcarPendiente();

        await uow.IntentosPago.AddAsync(intento, ct);
        await uow.SaveChangesAsync(ct);

        return new CrearCheckoutPagoResponse(
            pago.Id,
            intento.Id,
            checkout.CheckoutUrl,
            checkout.ExternalReference,
            checkout.ExpiraEn,
            cmd.MontoTotal,
            cmd.Moneda);
    }
}
