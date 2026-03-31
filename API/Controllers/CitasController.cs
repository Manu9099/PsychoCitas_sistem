using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Agenda.Queries;
using PsychoCitas.Application.Features.Citas.Commands;
using PsychoCitas.Application.Features.Citas.Queries;

namespace PsychoCitas.API.Controllers;

/// <summary>
/// Controlador para gestión de citas
/// Requiere autorización con política "Staff"
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Staff")]
public class CitasController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtiene todas las citas registradas
    /// GET /api/citas
    /// </summary>

    /// <summary>
    /// Obtiene la agenda de hoy (todas las citas del día actual)
    /// GET /api/citas/agenda/hoy
    /// </summary>
    [HttpGet("agenda/hoy")]
    public async Task<IActionResult> AgendaHoy([FromQuery] Guid? psicologoId, CancellationToken ct)
        => Ok(await mediator.Send(new GetAgendaHoyQuery(psicologoId), ct));

    /// <summary>
    /// Obtiene citas de una fecha específica
    /// GET /api/citas/agenda?fecha=2025-04-01
    /// </summary>
  

    /// <summary>
    /// Obtiene una cita por ID
    /// GET /api/citas/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetCitaByIdQuery(id), ct));

    /// <summary>
    /// Crea una nueva cita
    /// POST /api/citas
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Agendar([FromBody] AgendarCitaCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cancela una cita existente
    /// PATCH /api/citas/{id}/cancelar
    /// </summary>
    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(
        Guid id,
        [FromBody] CancelarRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new CancelarCitaCommand(id, request.Motivo), ct);
        return NoContent();
    }

    /// <summary>
    /// Marca una cita como completada
    /// PATCH /api/citas/{id}/completar
    /// </summary>
    [HttpPatch("{id:guid}/completar")]
    public async Task<IActionResult> Completar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new CompletarCitaCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Marca una cita como "no asistió"
    /// PATCH /api/citas/{id}/no-asistio
    /// </summary>
    [HttpPatch("{id:guid}/no-asistio")]
    public async Task<IActionResult> NoAsistio(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarcarNoAsistioCommand(id), ct);
        return NoContent();
    }
    
[HttpGet]
public async Task<IActionResult> GetAll(CancellationToken ct)
    => Ok(await mediator.Send(new GetAllCitasQuery(), ct));

}


/// <summary>
/// DTO para solicitud de cancelación
/// </summary>
public record CancelarRequest(string Motivo);