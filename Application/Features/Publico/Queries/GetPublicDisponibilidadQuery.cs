using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Publico.Queries;

public record GetPublicDisponibilidadQuery(
    Guid PsicologoId,
    DateOnly Fecha,
    int DuracionMinutos = 50
) : IRequest<PublicDisponibilidadDto>;

public class GetPublicDisponibilidadHandler(IUnitOfWork uow)
    : IRequestHandler<GetPublicDisponibilidadQuery, PublicDisponibilidadDto>
{
    public async Task<PublicDisponibilidadDto> Handle(
        GetPublicDisponibilidadQuery query,
        CancellationToken ct)
    {
        var inicioDia = query.Fecha.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Local).ToUniversalTime();
        var finDia = query.Fecha.ToDateTime(new TimeOnly(20, 0), DateTimeKind.Local).ToUniversalTime();

        var agenda = await uow.Citas.GetAgendaDiaAsync(query.PsicologoId, query.Fecha, ct);

        var ocupados = agenda
            .Where(c => c.Estado != PsychoCitas.Domain.Enums.EstadoCita.Cancelada &&
                        c.Estado != PsychoCitas.Domain.Enums.EstadoCita.NoAsistio)
            .Select(c => (c.FechaInicio, c.FechaFin))
            .OrderBy(x => x.FechaInicio)
            .ToList();

        var duracion = TimeSpan.FromMinutes(query.DuracionMinutos);
        var slots = new List<PublicSlotDto>();

        for (var actual = inicioDia; actual.Add(duracion) <= finDia; actual = actual.AddMinutes(30))
        {
            var slotFin = actual.Add(duracion);

            var solapa = ocupados.Any(o => actual < o.FechaFin && slotFin > o.FechaInicio);

            if (!solapa)
                slots.Add(new PublicSlotDto(actual, slotFin, true));
        }

        return new PublicDisponibilidadDto(
            query.PsicologoId,
            query.Fecha,
            query.DuracionMinutos,
            slots);
    }
}