using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Documentos.Queries;

public record DescargarDocumentoPacienteQuery(Guid DocumentoId) : IRequest<DocumentoDescargaDto>;

public class DescargarDocumentoPacienteHandler(
    IUnitOfWork uow,
    IStorageService storageService)
    : IRequestHandler<DescargarDocumentoPacienteQuery, DocumentoDescargaDto>
{
    public async Task<DocumentoDescargaDto> Handle(DescargarDocumentoPacienteQuery query, CancellationToken ct)
    {
        var documento = await uow.DocumentosPaciente.GetActivoByIdAsync(query.DocumentoId, ct)
            ?? throw new NotFoundException(nameof(DocumentoPaciente), query.DocumentoId);

        var stream = await storageService.ObtenerArchivoAsync(documento.UrlStorage, ct);

        return new DocumentoDescargaDto(
            documento.NombreOriginal,
            documento.ContentType,
            stream);
    }
}