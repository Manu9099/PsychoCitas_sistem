using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Notas.Commands;

public record GuardarNotaCommand(
    Guid CitaId,
    string? ResumenSesion,
    List<string>? TecnicasUsadas,
    int? EstadoAnimo,
    int? NivelAnsiedad,
    string? AvanceObjetivos,
    string? TareasAsignadas,
    string? Observaciones,
    string? PlanProximaSesion,
    bool EvaluacionRiesgo = false,
    NivelRiesgo? NivelRiesgo = null,
    string? AccionesRiesgo = null,
    bool Finalizar = false
) : IRequest<NotaSesionDto>;

public class GuardarNotaValidator : AbstractValidator<GuardarNotaCommand>
{
    public GuardarNotaValidator()
    {
        RuleFor(x => x.CitaId).NotEmpty();
        RuleFor(x => x.EstadoAnimo).InclusiveBetween(1, 10).When(x => x.EstadoAnimo.HasValue);
        RuleFor(x => x.NivelAnsiedad).InclusiveBetween(0, 10).When(x => x.NivelAnsiedad.HasValue);
        RuleFor(x => x.AccionesRiesgo).NotEmpty().When(x => x.EvaluacionRiesgo)
            .WithMessage("Debe describir las acciones ante el riesgo detectado.");
        RuleFor(x => x.ResumenSesion).NotEmpty().When(x => x.Finalizar)
            .WithMessage("El resumen es obligatorio para finalizar la nota.");
    }
}

public class GuardarNotaHandler(IUnitOfWork uow, ICurrentUser currentUser)
    : IRequestHandler<GuardarNotaCommand, NotaSesionDto>
{
    public async Task<NotaSesionDto> Handle(GuardarNotaCommand cmd, CancellationToken ct)
    {
        var cita = await uow.Citas.GetByIdAsync(cmd.CitaId, ct)
            ?? throw new NotFoundException(nameof(Cita), cmd.CitaId);

        // Obtener o crear nota
        NotaSesion nota;
        if (cita.Nota is null)
        {
            nota = NotaSesion.Crear(cmd.CitaId, currentUser.UserId);
            // Se agrega a través del repositorio en una implementación real
        }
        else
        {
            nota = cita.Nota;
        }

        nota.Actualizar(cmd.ResumenSesion, cmd.TecnicasUsadas,
            cmd.EstadoAnimo, cmd.NivelAnsiedad,
            cmd.AvanceObjetivos, cmd.TareasAsignadas,
            cmd.Observaciones, cmd.PlanProximaSesion);

        if (cmd.EvaluacionRiesgo && cmd.NivelRiesgo.HasValue)
            nota.RegistrarRiesgo(cmd.NivelRiesgo.Value, cmd.AccionesRiesgo!);

        if (cmd.Finalizar)
            nota.Finalizar();

        await uow.SaveChangesAsync(ct);

        return new NotaSesionDto(nota.Id, nota.CitaId, nota.ResumenSesion,
            nota.TecnicasUsadas, nota.EstadoAnimo, nota.NivelAnsiedad,
            nota.AvanceObjetivos, nota.TareasAsignadas, nota.PlanProximaSesion,
            nota.EvaluacionRiesgo, nota.NivelRiesgo, nota.Finalizada, nota.ActualizadoEn);
    }
}
