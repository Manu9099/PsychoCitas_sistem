using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Application.Features.Pagos.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Staff")]
public class PagosController(IMediator mediator) : ControllerBase
{
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPagoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("exonerar")]
    public async Task<IActionResult> Exonerar([FromBody] ExonerarPagoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("cita/{citaId:guid}")]
    public async Task<IActionResult> GetByCita(Guid citaId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPagoByCitaIdQuery(citaId), ct));

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPagosByPacienteIdQuery(pacienteId), ct));
}