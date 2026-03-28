namespace PsychoCitas.Application.Common.Interfaces;

public interface INotificationService
{
    Task EnviarEmailAsync(string email, string asunto, string contenido, CancellationToken ct = default);
    Task EnviarWhatsAppAsync(string telefono, string mensaje, CancellationToken ct = default);
    Task EnviarSmsAsync(string telefono, string mensaje, CancellationToken ct = default);
}