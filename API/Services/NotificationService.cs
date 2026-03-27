using PsychoCitas.Application.Common.Interfaces;

namespace PsychoCitas.API.Services;

public class NotificationService : INotificationService
{
    public Task EnviarEmailAsync(string destino, string asunto, string mensaje, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task EnviarWhatsAppAsync(string telefono, string mensaje, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task EnviarSmsAsync(string telefono, string mensaje, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}