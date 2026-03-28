using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Notas.Queries;

public record GetNotaByCitaIdQuery(Guid CitaId) : IRequest<NotaSesionDto>;

public class GetNotaByCitaIdHandler(IUnitOfWork uow) : IRequestHandler<GetNotaByCitaIdQuery, NotaSesionDto>
{
    public async Task<NotaSesionDto> Handle(GetNotaByCitaIdQuery query, CancellationToken ct)
    {
        var nota = await uow.Notas.GetByCitaIdAsync(query.CitaId, ct)
            ?? throw new NotFoundException(nameof(NotaSesion), query.CitaId);

        return new NotaSesionDto(
            nota.Id,
            nota.CitaId,
            nota.ResumenSesion,
            nota.TecnicasUsadas,
            nota.EstadoAnimo,
            nota.NivelAnsiedad,
            nota.AvanceObjetivos,
            nota.TareasAsignadas,
            nota.PlanProximaSesion,
            nota.EvaluacionRiesgo,
            nota.NivelRiesgo,
            nota.Finalizada,
            nota.ActualizadoEn
        );
    }
}
