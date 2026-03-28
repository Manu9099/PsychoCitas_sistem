using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class ReminderWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ReminderWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan Horizon = TimeSpan.FromHours(25);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ReminderWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarCicloAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en ReminderWorker.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task ProcesarCicloAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var ahora = DateTime.UtcNow;

        var citas = await uow.Citas.GetProximasConRecordatorioAsync(
            ahora,
            ahora.Add(Horizon),
            ct);

        foreach (var cita in citas)
        {
            if (cita.Paciente is null)
                continue;

            await ProgramarSiNoExisteAsync(
                uow,
                cita,
                "recordatorio_24h",
                cita.FechaInicio.AddHours(-24),
                ct);

            await ProgramarSiNoExisteAsync(
                uow,
                cita,
                "recordatorio_2h",
                cita.FechaInicio.AddHours(-2),
                ct);
        }

        await uow.SaveChangesAsync(ct);

        var pendientes = await uow.Notificaciones.GetPendientesConDestinoAsync(ahora, ct);

        foreach (var notificacion in pendientes)
        {
            try
            {
                await EnviarAsync(notificacion, notificationService, ct);
                notificacion.MarcarEnviada();
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error enviando {Canal} {Tipo} para cita {CitaId}",
                    notificacion.Canal,
                    notificacion.Tipo,
                    notificacion.CitaId);

                notificacion.MarcarFallida(ex.Message);
            }
        }

        await uow.SaveChangesAsync(ct);
    }

    private async Task ProgramarSiNoExisteAsync(
        IUnitOfWork uow,
        Cita cita,
        string tipo,
        DateTime programadaPara,
        CancellationToken ct)
    {
        if (cita.Paciente is null)
            return;

        foreach (var canal in ObtenerCanales(cita.Paciente))
        {
            var existe = await uow.Notificaciones.ExisteDuplicadoAsync(
                cita.Id,
                canal,
                tipo,
                ct);

            if (existe)
                continue;

            var mensaje = ConstruirMensaje(cita.Paciente, cita, tipo);

            var notificacion = Notificacion.Crear(
                cita.Id,
                cita.PacienteId,
                canal,
                tipo,
                programadaPara,
                mensaje);

            await uow.Notificaciones.AddAsync(notificacion, ct);
        }
    }

    private static IEnumerable<string> ObtenerCanales(Paciente paciente)
    {
        if (!string.IsNullOrWhiteSpace(paciente.Email))
            yield return "email";

        if (!string.IsNullOrWhiteSpace(paciente.Telefono))
            yield return "whatsapp";

        // Si luego quieres activar SMS automático, descomenta esto:
        // if (!string.IsNullOrWhiteSpace(paciente.Telefono))
        //     yield return "sms";
    }

    private static string ConstruirMensaje(Paciente paciente, Cita cita, string tipo)
    {
        var etiqueta = tipo == "recordatorio_24h" ? "mañana" : "en 2 horas";

        return
            $"Hola {paciente.Nombres}, te recordamos que tienes una cita {etiqueta}, " +
            $"el {cita.FechaInicio:dd/MM/yyyy} a las {cita.FechaInicio:HH:mm}.";
    }

    private static string ConstruirAsunto(string tipo)
        => tipo == "recordatorio_24h"
            ? "Recordatorio de cita - 24 horas"
            : "Recordatorio de cita - 2 horas";

    private static Task EnviarAsync(
        Notificacion notificacion,
        INotificationService notificationService,
        CancellationToken ct)
    {
        var paciente = notificacion.Cita?.Paciente
            ?? throw new InvalidOperationException("La notificación no tiene paciente relacionado.");

        return notificacion.Canal switch
        {
            "email" when !string.IsNullOrWhiteSpace(paciente.Email)
                => notificationService.EnviarEmailAsync(
                    paciente.Email,
                    ConstruirAsunto(notificacion.Tipo),
                    notificacion.Mensaje ?? string.Empty,
                    ct),

            "whatsapp" when !string.IsNullOrWhiteSpace(paciente.Telefono)
                => notificationService.EnviarWhatsAppAsync(
                    paciente.Telefono,
                    notificacion.Mensaje ?? string.Empty,
                    ct),

            "sms" when !string.IsNullOrWhiteSpace(paciente.Telefono)
                => notificationService.EnviarSmsAsync(
                    paciente.Telefono,
                    notificacion.Mensaje ?? string.Empty,
                    ct),

            _ => throw new InvalidOperationException(
                $"Canal no soportado o destino faltante: {notificacion.Canal}")
        };
    }
}