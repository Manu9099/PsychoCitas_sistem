using FluentValidation;
using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Commands;

public record ExonerarPagoCommand(
    Guid CitaId,
    decimal MontoTotal,
    string Motivo
) : IRequest<PagoDto>;

public class ExonerarPagoValidator : AbstractValidator<ExonerarPagoCommand>
{
    public ExonerarPagoValidator()
    {
        RuleFor(x => x.CitaId).NotEmpty();
        RuleFor(x => x.MontoTotal).GreaterThan(0);
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(500);
    }
}

public class ExonerarPagoHandler(IUnitOfWork uow)
    : IRequestHandler<ExonerarPagoCommand, PagoDto>
{
    public async Task<PagoDto> Handle(ExonerarPagoCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);

        var pago = await uow.Pagos.GetByCitaIdAsync(cmd.CitaId, ct);

        if (pago is null)
        {
            pago = Pago.Crear(cmd.CitaId, cita.PacienteId, cmd.MontoTotal);
            await uow.Pagos.AddAsync(pago, ct);
        }

        pago.Exonerar(cmd.Motivo);
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