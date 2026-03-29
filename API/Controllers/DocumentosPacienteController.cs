using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.API.Models;
using PsychoCitas.Application.Features.Documentos.Commands;
using PsychoCitas.Application.Features.Documentos.Queries;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/pacientes/{pacienteId:guid}/documentos")]
[Authorize(Policy = "Staff")]
public class DocumentosPacienteController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(Guid pacienteId, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarDocumentosPacienteQuery(pacienteId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Subir(
        Guid pacienteId,
        [FromForm] SubirDocumentoPacienteRequest request,
        CancellationToken ct)
    {
        if (request.Archivo is null || request.Archivo.Length == 0)
            return BadRequest("Archivo requerido.");

        await using var ms = new MemoryStream();
        await request.Archivo.CopyToAsync(ms, ct);

        var command = new SubirDocumentoPacienteCommand(
            pacienteId,
            request.Tipo,
            request.Archivo.FileName,
            request.Archivo.ContentType,
            ms.ToArray(),
            request.Observaciones);

        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Listar), new { pacienteId }, result);
    }

    [HttpGet("{documentoId:guid}/download")]
    public async Task<IActionResult> Descargar(Guid pacienteId, Guid documentoId, CancellationToken ct)
    {
        var result = await mediator.Send(new DescargarDocumentoPacienteQuery(documentoId), ct);
        return File(result.Contenido, result.ContentType, result.NombreOriginal);
    }
}