using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Agenda.Queries;

public record GetAgendaHoyQuery(Guid? PsicologoId = null) : IRequest<IEnumerable<CitaDto>>;

public class GetAgendaHoyHandler(IUnitOfWork uow, ICurrentUser currentUser)
    : IRequestHandler<GetAgendaHoyQuery, IEnumerable<CitaDto>>
{
    public async Task<IEnumerable<CitaDto>> Handle(GetAgendaHoyQuery query, CancellationToken ct)
    {
        var psicologoId = query.PsicologoId ?? currentUser.UserId;
        var citas = await uow.Citas.GetAgendaDiaAsync(psicologoId, DateOnly.FromDateTime(DateTime.Today), ct);

        return citas.Select(c => new CitaDto(
            c.Id, c.PacienteId,
            c.Paciente?.NombreCompleto ?? string.Empty,
            c.Paciente?.Telefono,
            c.PsicologoId, c.FechaInicio, c.FechaFin,
            c.TipoSesion, c.Modalidad, c.Estado,
            c.LinkVideollamada, c.NumeroSesion, c.EsPrimeraVez, c.NotasPrevias,
            c.MotivoCancelacion,
            c.Pago?.Estado, c.Pago?.Monto,
            c.Nota is not null));
    }
}
