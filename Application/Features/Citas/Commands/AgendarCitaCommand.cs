using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Citas.Commands;

// ── Command ──
public record AgendarCitaCommand(
    Guid PacienteId,
    Guid PsicologoId,
    DateTime FechaInicio,
    DateTime FechaFin,
    TipoSesion TipoSesion,
    Modalidad Modalidad,
    bool EsPrimeraVez = false,
    string? LinkVideollamada = null,
    string? NotasPrevias = null
) : IRequest<CitaResumenDto>;

// ── Validator ──
public class AgendarCitaValidator : AbstractValidator<AgendarCitaCommand>
{
    
    public AgendarCitaValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty().WithMessage("El paciente es requerido.");
        RuleFor(x => x.PsicologoId).NotEmpty().WithMessage("El psicólogo es requerido.");
        RuleFor(x => x.FechaInicio).GreaterThan(DateTime.UtcNow).WithMessage("La fecha debe ser futura.");
        RuleFor(x => x.FechaFin).GreaterThan(x => x.FechaInicio).WithMessage("La fecha fin debe ser posterior al inicio.");
        RuleFor(x => x).Must(x => (x.FechaFin - x.FechaInicio).TotalMinutes is >= 30 and <= 120)
            .WithMessage("La sesión debe durar entre 30 y 120 minutos.");
        RuleFor(x => x.LinkVideollamada)
            .NotEmpty().When(x => x.Modalidad == Modalidad.Videollamada)
            .WithMessage("El link de videollamada es requerido para sesiones virtuales.");
    }
}

// ── Handler ──
public class AgendarCitaHandler(
    
    IUnitOfWork uow,
    ICurrentUser currentUser,
    INotificationService notificationService) : IRequestHandler<AgendarCitaCommand, CitaResumenDto>
{
    public async Task<CitaResumenDto> Handle(AgendarCitaCommand cmd, CancellationToken ct)
    
    {
        // Verificar solapamiento
        var hayConflicto = await uow.Citas.ExisteSolapamientoAsync(cmd.PsicologoId, cmd.FechaInicio, cmd.FechaFin, ct: ct);
        if (hayConflicto)
            throw new ConflictoAgendaException(cmd.FechaInicio, cmd.FechaFin);
            
       //verficar que le psicologo que se le asigna la cita es el mismo que esta logueado
          if (currentUser.EsPsicologo && cmd.PsicologoId != currentUser.UserId)
        throw new UnauthorizedAccessException("No puedes agendar citas para otro psicólogo.");
 
        // Verificar que el paciente existe
        var paciente = await uow.Pacientes.GetByIdAsync(cmd.PacienteId, ct)
            ?? throw new NotFoundException(nameof(Paciente), cmd.PacienteId);

        // Crear la cita
        var cita = Cita.Crear(
            cmd.PacienteId, cmd.PsicologoId,
            cmd.FechaInicio, cmd.FechaFin,
            cmd.TipoSesion, cmd.Modalidad,
            cmd.EsPrimeraVez, cmd.LinkVideollamada,
            cmd.NotasPrevias, currentUser.UserId);

        await uow.Citas.AddAsync(cita, ct);

        // Crear pago pendiente automáticamente (si se desea)
        // ... lógica de pago

        await uow.SaveChangesAsync(ct);

        // Notificación de confirmación (fire & forget con await para no bloquear)
        _ = EnviarConfirmacionAsync(paciente, cita);

        return new CitaResumenDto(
            cita.Id, cita.FechaInicio, cita.FechaFin,
            paciente.NombreCompleto, cita.Estado,
            cita.Modalidad, cita.TipoSesion, cita.EsPrimeraVez);
    }

    private async Task EnviarConfirmacionAsync(Paciente paciente, Cita cita)
    {
        if (paciente.Email is null) return;
        await notificationService.EnviarEmailAsync(
            paciente.Email,
            "Confirmación de cita",
            $"Hola {paciente.Nombres}, tu cita está agendada para el {cita.FechaInicio:dddd dd/MM/yyyy} a las {cita.FechaInicio:HH:mm}."
        );
    }
}
