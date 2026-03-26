using MediatR;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Commands;

public record CompletarCitaCommand(Guid CitaId) : IRequest<Unit>;

public class CompletarCitaHandler(IUnitOfWork uow) : IRequestHandler<CompletarCitaCommand, Unit>
{
    public async Task<Unit> Handle(CompletarCitaCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);
        var sesiones = await uow.Citas.ContarSesionesCompletadasAsync(cita.PacienteId, cita.PsicologoId, ct);
        cita.Completar(sesiones + 1);
        uow.Citas.Update(cita);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
