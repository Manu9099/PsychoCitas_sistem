using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Commands;

public record ConfirmarPagoIntentoCommand(
    string ExternalReference,
    decimal MontoPagado,
    string MetodoPago,
    string? NumeroOperacion,
    string PayloadRaw
) : IRequest<PagoDto>;

public class ConfirmarPagoIntentoHandler(IUnitOfWork uow)
    : IRequestHandler<ConfirmarPagoIntentoCommand, PagoDto>
{
    public async Task<PagoDto> Handle(ConfirmarPagoIntentoCommand cmd, CancellationToken ct)
    {
        var intento = await uow.IntentosPago.GetByExternalReferenceAsync(cmd.ExternalReference, ct)
            ?? throw new NotFoundException("IntentoPago", cmd.ExternalReference);

        var pago = intento.Pago ?? throw new DomainException("Intento sin pago asociado.");

        var evento = Domain.Entities.EventoPago.Crear(
            intento.Id,
            intento.Proveedor,
            "payment.confirmed",
            cmd.PayloadRaw,
            providerEventId: $"{cmd.ExternalReference}-confirmed");

        await uow.EventosPago.AddAsync(evento, ct);

        intento.MarcarExitoso(cmd.NumeroOperacion, cmd.PayloadRaw);

        pago.Registrar(
            cmd.MontoPagado,
            cmd.MetodoPago,
            cmd.NumeroOperacion,
            "Confirmado desde callback/mock");

        await uow.SaveChangesAsync(ct);

        return new PagoDto(
            pago.Id,
            pago.CitaId,
            pago.PacienteId,
            pago.Monto,
            pago.MontoPagado,
            pago.Saldo,
            pago.Estado,
            pago.MetodoPago,
            pago.NumeroOperacion,
            pago.Notas,
            pago.RegistradoPor,
            pago.PagadoEn,
            pago.CreadoEn);
    }
}
