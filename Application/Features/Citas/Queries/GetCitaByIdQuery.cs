using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Queries;

public record GetCitaByIdQuery(Guid CitaId) : IRequest<CitaDto>;

public class GetCitaByIdHandler(IUnitOfWork uow) : IRequestHandler<GetCitaByIdQuery, CitaDto>
{
    public async Task<CitaDto> Handle(GetCitaByIdQuery query, CancellationToken ct)
    {
        var cita = await uow.Citas.GetDetalleByIdAsync(query.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), query.CitaId);

        return new CitaDto(
            cita.Id,
            cita.PacienteId,
            cita.Paciente?.NombreCompleto ?? string.Empty,
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
            cita.Nota is not null
        );
    }
}