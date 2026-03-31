using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Queries;

public record GetAllCitasQuery() : IRequest<List<CitaDto>>;

public class GetAllCitasHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllCitasQuery, List<CitaDto>>
{
    public async Task<List<CitaDto>> Handle(GetAllCitasQuery query, CancellationToken ct)
    {
        var citas = await uow.Citas.GetAllConDetallesAsync(ct);

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
