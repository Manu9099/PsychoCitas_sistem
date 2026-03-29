using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Documentos.Queries;

public record ListarDocumentosPacienteQuery(Guid PacienteId) : IRequest<List<DocumentoPacienteDto>>;

public class ListarDocumentosPacienteHandler(IUnitOfWork uow)
    : IRequestHandler<ListarDocumentosPacienteQuery, List<DocumentoPacienteDto>>
{
    public async Task<List<DocumentoPacienteDto>> Handle(ListarDocumentosPacienteQuery query, CancellationToken ct)
    {
        var documentos = await uow.DocumentosPaciente.GetByPacienteIdAsync(query.PacienteId, ct);

        return documentos
            .Select(d => new DocumentoPacienteDto(
                d.Id,
                d.PacienteId,
                d.Tipo,
                d.NombreOriginal,
                d.ContentType,
                d.Extension,
                d.TamanoBytes,
                d.CreadoEn,
                d.Observaciones))
            .ToList();
    }
}