using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Publico.Commands;

public record CrearCitaPublicaCommand(
    string Nombres,
    string Apellidos,
    string? Email,
    string? Telefono,
    string? Dni,
    Guid PsicologoId,
    DateTime FechaInicio,
    DateTime FechaFin,
    TipoSesion TipoSesion,
    Modalidad Modalidad,
    bool EsPrimeraVez = true,
    string? LinkVideollamada = null,
    string? NotasPrevias = null,
    string? ReferidoPor = "Web pública"
) : IRequest<PublicCitaCreadaDto>;

public class CrearCitaPublicaValidator : AbstractValidator<CrearCitaPublicaCommand>
{
    public CrearCitaPublicaValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Telefono))
            .WithMessage("Debes enviar al menos email o teléfono.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.FechaInicio)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio);

        RuleFor(x => x)
            .Must(x => (x.FechaFin - x.FechaInicio).TotalMinutes is >= 30 and <= 120)
            .WithMessage("La sesión debe durar entre 30 y 120 minutos.");

        RuleFor(x => x.LinkVideollamada)
            .NotEmpty()
            .When(x => x.Modalidad == Modalidad.Videollamada);
    }
}

public class CrearCitaPublicaHandler(
  IUnitOfWork uow,
    INotificationService notificationService,
    ILogger<CrearCitaPublicaHandler> logger)
    : IRequestHandler<CrearCitaPublicaCommand, PublicCitaCreadaDto>
{
    public async Task<PublicCitaCreadaDto> Handle(CrearCitaPublicaCommand cmd, CancellationToken ct)
    {
        var hayConflicto = await uow.Citas.ExisteSolapamientoAsync(
            cmd.PsicologoId,
            cmd.FechaInicio,
            cmd.FechaFin,
            ct: ct);

        if (hayConflicto)
            throw new ConflictoAgendaException(cmd.FechaInicio, cmd.FechaFin);

        Paciente? paciente = null;

        if (!string.IsNullOrWhiteSpace(cmd.Dni))
            paciente = await uow.Pacientes.GetByDniAsync(cmd.Dni, ct);

        if (paciente is null && !string.IsNullOrWhiteSpace(cmd.Email))
            paciente = await uow.Pacientes.GetByEmailAsync(cmd.Email, ct);

        if (paciente is null)
        {
            paciente = Paciente.Create(
                cmd.Nombres,
                cmd.Apellidos,
                cmd.Email,
                cmd.Telefono,
                cmd.Dni,
                null,
                null,
                null,
                cmd.ReferidoPor);

            await uow.Pacientes.AddAsync(paciente, ct);
            await uow.SaveChangesAsync(ct);
        }

        var cita = Cita.Crear(
            paciente.Id,
            cmd.PsicologoId,
            cmd.FechaInicio,
            cmd.FechaFin,
            cmd.TipoSesion,
            cmd.Modalidad,
            cmd.EsPrimeraVez,
            cmd.LinkVideollamada,
            cmd.NotasPrevias,
            null);

       await uow.Citas.AddAsync(cita, ct);
await uow.SaveChangesAsync(ct);

if (!string.IsNullOrWhiteSpace(paciente.Email))
{
    try
    {
        await notificationService.EnviarEmailAsync(
            paciente.Email,
            "Confirmación de cita",
            $"Hola {paciente.Nombres}, tu cita fue reservada para el {cita.FechaInicio:dd/MM/yyyy} a las {cita.FechaInicio:HH:mm}.",
            ct);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex,
            "No se pudo enviar email de confirmación para la cita pública {CitaId}",
            cita.Id);
    }
}

return new PublicCitaCreadaDto(
    cita.Id,
    paciente.Id,
    "Cita reservada correctamente.",
    cita.FechaInicio,
    cita.FechaFin,
    paciente.Email);
    }
    }