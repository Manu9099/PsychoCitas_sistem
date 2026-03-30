using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Queries;

public record GetPagosByPacienteIdQuery(Guid PacienteId) : IRequest<List<PagoResumenPacienteDto>>;

public class GetPagosByPacienteIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPagosByPacienteIdQuery, List<PagoResumenPacienteDto>>
{
    public async Task<List<PagoResumenPacienteDto>> Handle(GetPagosByPacienteIdQuery query, CancellationToken ct)
    {
        var pagos = await uow.Pagos.GetByPacienteIdAsync(query.PacienteId, ct);

        return pagos.Select(p => new PagoResumenPacienteDto(
            p.Id,
            p.CitaId,
            p.Monto,
            p.MontoPagado,
            p.Saldo,
            p.Estado,
            p.MetodoPago,
            p.PagadoEn,
            p.Cita?.FechaInicio ?? p.CreadoEn
        )).ToList();
    }
}