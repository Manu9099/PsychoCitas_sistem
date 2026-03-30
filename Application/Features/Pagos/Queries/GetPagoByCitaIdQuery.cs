using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pagos.Queries;

public record GetPagoByCitaIdQuery(Guid CitaId) : IRequest<PagoDto>;

public class GetPagoByCitaIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPagoByCitaIdQuery, PagoDto>
{
    public async Task<PagoDto> Handle(GetPagoByCitaIdQuery query, CancellationToken ct)
    {
        var pago = await uow.Pagos.GetByCitaIdAsync(query.CitaId, ct)
            ?? throw new NotFoundException(nameof(Pago), query.CitaId);

        return new PagoDto(
            pago.Id,
            pago.CitaId,
            pago.PacienteId,
            pago.Monto,
            pago.MontoPagado,
            pago.Saldo,
            pago.Estado,
            pago.MetodoPago,
            pago.NumeroOperacion,
            pago.Notas,
            pago.RegistradoPor,
            pago.PagadoEn,
            pago.CreadoEn);
    }
}