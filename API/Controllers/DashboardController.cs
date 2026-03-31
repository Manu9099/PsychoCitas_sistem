using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Dashboard.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Staff")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/dashboard/resumen
    /// Retorna resumen del dashboard: citas de hoy, pacientes totales, total pagado
    /// </summary>
    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen(CancellationToken ct)
        => Ok(await mediator.Send(new GetDashboardResumenQuery(), ct));
}