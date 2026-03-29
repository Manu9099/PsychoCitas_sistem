using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Documentos.Commands;

public record SubirDocumentoPacienteCommand(
    Guid PacienteId,
    TipoDocumentoPaciente Tipo,
    string NombreOriginal,
    string ContentType,
    byte[] Contenido,
    string? Observaciones = null
) : IRequest<DocumentoPacienteDto>;

public class SubirDocumentoPacienteValidator : AbstractValidator<SubirDocumentoPacienteCommand>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"];
    private const int MaxSizeBytes = 10 * 1024 * 1024;

    public SubirDocumentoPacienteValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty();
        RuleFor(x => x.NombreOriginal).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Contenido).NotNull().Must(x => x.Length > 0).WithMessage("El archivo está vacío.");
        RuleFor(x => x.Contenido.Length).LessThanOrEqualTo(MaxSizeBytes);

        RuleFor(x => x.NombreOriginal)
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage("Extensión de archivo no permitida.");
    }
}

public class SubirDocumentoPacienteHandler(
    IUnitOfWork uow,
    IStorageService storageService)
    : IRequestHandler<SubirDocumentoPacienteCommand, DocumentoPacienteDto>
{
    public async Task<DocumentoPacienteDto> Handle(SubirDocumentoPacienteCommand cmd, CancellationToken ct)
    {
        var paciente = await uow.Pacientes.GetByIdAsync(cmd.PacienteId, ct)
            ?? throw new NotFoundException(nameof(Paciente), cmd.PacienteId);

        var extension = Path.GetExtension(cmd.NombreOriginal).ToLowerInvariant();
        var nombreArchivo = $"{cmd.PacienteId}/{Guid.NewGuid()}{extension}";

        await using var ms = new MemoryStream(cmd.Contenido);
        var url = await storageService.SubirArchivoAsync(ms, nombreArchivo, cmd.ContentType, ct);

        var documento = DocumentoPaciente.Crear(
            cmd.PacienteId,
            cmd.Tipo,
            cmd.NombreOriginal,
            nombreArchivo,
            cmd.ContentType,
            cmd.Contenido.LongLength,
            url,
            cmd.Observaciones);

        await uow.DocumentosPaciente.AddAsync(documento, ct);
        await uow.SaveChangesAsync(ct);

        return new DocumentoPacienteDto(
            documento.Id,
            documento.PacienteId,
            documento.Tipo,
            documento.NombreOriginal,
            documento.ContentType,
            documento.Extension,
            documento.TamanoBytes,
            documento.CreadoEn,
            documento.Observaciones);
    }
}