using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Agenda.Queries;
using PsychoCitas.Application.Features.Citas.Commands;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitasController(IMediator mediator) : ControllerBase
{
    /// <summary>Agenda del día del psicólogo</summary>
    [HttpGet("agenda/hoy")]
    public async Task<IActionResult> AgendaHoy([FromQuery] Guid? psicologoId, CancellationToken ct)
        => Ok(await mediator.Send(new GetAgendaHoyQuery(psicologoId), ct));

    /// <summary>Agendar una nueva cita</summary>
    [HttpPost]
    public async Task<IActionResult> Agendar([FromBody] AgendarCitaCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Obtener cita por ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        // TODO: Add GetCitaByIdQuery
        return Ok();
    }

    /// <summary>Cancelar una cita</summary>
    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarRequest request, CancellationToken ct)
    {
        await mediator.Send(new CancelarCitaCommand(id, request.Motivo), ct);
        return NoContent();
    }

    /// <summary>Marcar como completada</summary>
    [HttpPatch("{id:guid}/completar")]
    public async Task<IActionResult> Completar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new CompletarCitaCommand(id), ct);
        return NoContent();
    }

    /// <summary>Marcar inasistencia</summary>
    [HttpPatch("{id:guid}/no-asistio")]
    public async Task<IActionResult> NoAsistio(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarcarNoAsistioCommand(id), ct);
        return NoContent();
    }
}

public record CancelarRequest(string Motivo);
