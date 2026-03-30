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

            return citas.Select(cita => new CitaDto(
                cita.Id,
                cita.PacienteId,
                $"{cita.Paciente?.Nombres} {cita.Paciente?.Apellidos}".Trim(),
                cita.Paciente?.Telefono,
                cita.PsicologoId,
                cita.FechaInicio,
                cita.FechaFin,
                cita.TipoSesion,
                cita.Modalidad,
                cita.Estado,
                cita.LinkVideollamada,
                cita.NumeroSesion,
                cita.EsPrimeraVez,
                cita.NotasPrevias,
                cita.MotivoCancelacion,
                cita.Pago?.Estado,
                cita.Pago?.Monto,
                cita.Pago?.MontoPagado,
                cita.Pago?.Saldo,
                cita.Pago?.MetodoPago,
                cita.Nota is not null
            )).ToList();
    }
}
