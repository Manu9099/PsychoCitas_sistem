using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Notas.Commands;
using PsychoCitas.Application.Features.Notas.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/citas/{citaId:guid}/nota")]
[Authorize]
public class NotasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obtener(Guid citaId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetNotaByCitaIdQuery(citaId), ct));

    [HttpPut]
    public async Task<IActionResult> Guardar(Guid citaId, [FromBody] GuardarNotaCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command with { CitaId = citaId }, ct);
        return Ok(result);
    }

    [HttpPatch("finalizar")]
    public async Task<IActionResult> Finalizar(Guid citaId, CancellationToken ct)
    {
        var result = await mediator.Send(
            new GuardarNotaCommand(citaId, null, null, null, null, null, null, null, null, Finalizar: true),
            ct);

        return Ok(result);
    }
}