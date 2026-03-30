using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Commands;

public record RegistrarPagoCommand(
    Guid CitaId,
    decimal MontoTotal,
    decimal MontoPagado,
    string MetodoPago,
    string? NumeroOperacion,
    string? Notas
) : IRequest<PagoDto>;

public class RegistrarPagoValidator : AbstractValidator<RegistrarPagoCommand>
{
    private static readonly string[] AllowedMethods =
    [
        "Yape", "Plin", "Efectivo", "Transferencia"
    ];

    public RegistrarPagoValidator()
    {
        RuleFor(x => x.CitaId).NotEmpty();
        RuleFor(x => x.MontoTotal).GreaterThan(0);
        RuleFor(x => x.MontoPagado).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MetodoPago)
            .NotEmpty()
            .Must(x => AllowedMethods.Contains(x))
            .WithMessage("Método de pago no válido.");
        RuleFor(x => x.NumeroOperacion)
            .MaximumLength(100);
        RuleFor(x => x.Notas)
            .MaximumLength(500);
    }
}

public class RegistrarPagoHandler(
    IUnitOfWork uow,
    ICurrentUser currentUser)
    : IRequestHandler<RegistrarPagoCommand, PagoDto>
{
    public async Task<PagoDto> Handle(RegistrarPagoCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);

        var pago = await uow.Pagos.GetByCitaIdAsync(cmd.CitaId, ct);

        if (pago is null)
        {
            pago = Pago.Crear(
                cmd.CitaId,
                cita.PacienteId,
                cmd.MontoTotal,
                currentUser.IsAuthenticated ? currentUser.UserId : null);

            await uow.Pagos.AddAsync(pago, ct);
        }
        else if (pago.Estado == PsychoCitas.Domain.Enums.EstadoPago.Exonerado)
        {
            throw new DomainException("La cita ya está exonerada.");
        }

        pago.Registrar(
            cmd.MontoPagado,
            cmd.MetodoPago,
            cmd.NumeroOperacion,
            cmd.Notas);

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