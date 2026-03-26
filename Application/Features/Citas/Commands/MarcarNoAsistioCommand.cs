using MediatR;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Commands;

public record MarcarNoAsistioCommand(Guid CitaId) : IRequest<Unit>;

public class MarcarNoAsistioHandler(IUnitOfWork uow) : IRequestHandler<MarcarNoAsistioCommand, Unit>
{
    public async Task<Unit> Handle(MarcarNoAsistioCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);
        cita.MarcarNoAsistio();
        uow.Citas.Update(cita);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
