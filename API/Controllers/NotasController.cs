using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Notas.Commands;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/citas/{citaId:guid}/nota")]
[Authorize]
public class NotasController(IMediator mediator) : ControllerBase
{
    /// <summary>Guardar o actualizar nota de sesión</summary>
    [HttpPut]
    public async Task<IActionResult> Guardar(Guid citaId, [FromBody] GuardarNotaCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command with { CitaId = citaId }, ct);
        return Ok(result);
    }

    /// <summary>Finalizar nota de sesión</summary>
    [HttpPatch("finalizar")]
    public async Task<IActionResult> Finalizar(Guid citaId, CancellationToken ct)
    {
        var result = await mediator.Send(new GuardarNotaCommand(citaId, null, null, null, null, null, null, null, null, Finalizar: true), ct);
        return Ok(result);
    }
}
