using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Pacientes.Commands;
using PsychoCitas.Application.Features.Pacientes.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PacientesController(IMediator mediator) : ControllerBase
{
    /// <summary>Detalle completo de un paciente</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetalle(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetPacienteDetalleQuery(id), ct));

    /// <summary>Buscar pacientes</summary>
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string termino, CancellationToken ct)
        => Ok(await mediator.Send(new BuscarPacientesQuery(termino), ct));

    /// <summary>Registrar nuevo paciente</summary>
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPacienteCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetDetalle), new { id = result.Id }, result);
    }

    
}
