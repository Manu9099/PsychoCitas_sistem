using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Commands;

public record CancelarCitaCommand(Guid CitaId, string Motivo) : IRequest<Unit>;

public class CancelarCitaValidator : AbstractValidator<CancelarCitaCommand>
{
    public CancelarCitaValidator()
    {
        RuleFor(x => x.CitaId).NotEmpty();
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(500)
            .WithMessage("Debe indicar un motivo de cancelación (máx. 500 caracteres).");
    }
}

public class CancelarCitaHandler(IUnitOfWork uow, ICurrentUser currentUser)
    : IRequestHandler<CancelarCitaCommand, Unit>
{
    public async Task<Unit> Handle(CancelarCitaCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);

        cita.Cancelar(cmd.Motivo, currentUser.UserId);
        uow.Citas.Update(cita);
        await uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
