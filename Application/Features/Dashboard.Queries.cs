using MediatR;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Dashboard.Queries;

public record DashboardResumenDto(
    int CitasHoy,
    int PacientesTotales,
    decimal TotalPagado
);

public record GetDashboardResumenQuery() : IRequest<DashboardResumenDto>;

public class GetDashboardResumenHandler(IUnitOfWork uow)
    : IRequestHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    public async Task<DashboardResumenDto> Handle(GetDashboardResumenQuery query, CancellationToken ct)
    {
        var citas = await uow.Citas.GetAllAsync(ct);
        var pacientes = await uow.Pacientes.GetAllAsync(ct);
        var pagos = await uow.Pagos.GetAllAsync(ct);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var citasHoy = citas.Count(c =>
            DateOnly.FromDateTime(c.FechaInicio) == hoy);

        var pacientesTotales = pacientes.Count(p => p.Activo);

        var totalPagado = pagos
            .Where(p => p.Estado == EstadoPago.Pagado || p.Estado == EstadoPago.Parcial)
            .Sum(p => p.MontoPagado);

        return new DashboardResumenDto(
            CitasHoy: citasHoy,
            PacientesTotales: pacientesTotales,
            TotalPagado: totalPagado
        );
    }
}