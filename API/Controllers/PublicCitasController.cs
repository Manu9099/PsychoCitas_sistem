using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Application.Features.Publico.Commands;
using PsychoCitas.Application.Features.Publico.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
[EnableRateLimiting("PublicBooking")]
public class PublicCitasController(IMediator mediator) : ControllerBase
{
    [HttpGet("disponibilidad")]
    public async Task<IActionResult> GetDisponibilidad(
        [FromQuery] Guid psicologoId,
        [FromQuery] DateOnly fecha,
        [FromQuery] int duracionMinutos = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetPublicDisponibilidadQuery(psicologoId, fecha, duracionMinutos),
            ct);

        return Ok(result);
    }

    [HttpPost("citas")]
    public async Task<IActionResult> CrearCitaPublica(
        [FromBody] CrearCitaPublicaCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(CrearCitaPublica), new { id = result.CitaId }, result);
    }
    [HttpPost("checkout")]
    public async Task<IActionResult> CrearCheckout([FromBody] CrearCheckoutPagoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("callback/mock")]
    public async Task<IActionResult> ConfirmarMock([FromBody] ConfirmarPagoIntentoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }
}