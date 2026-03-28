using Microsoft.Extensions.Logging;
using PsychoCitas.Application.Common.Interfaces;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly SendGridEmailSender _emailSender;
    private readonly TwilioSmsSender _smsSender;
    private readonly TwilioWhatsAppSender _whatsAppSender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        SendGridEmailSender emailSender,
        TwilioSmsSender smsSender,
        TwilioWhatsAppSender whatsAppSender,
        ILogger<NotificationService> logger)
    {
        _emailSender = emailSender;
        _smsSender = smsSender;
        _whatsAppSender = whatsAppSender;
        _logger = logger;
    }

    public async Task EnviarEmailAsync(string email, string asunto, string contenido, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando email a {Email} con asunto {Asunto}", email, asunto);
        await _emailSender.SendAsync(email, asunto, contenido, null, ct);
    }

    public async Task EnviarWhatsAppAsync(string telefono, string mensaje, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando WhatsApp a {Telefono}", telefono);
        await _whatsAppSender.SendAsync(telefono, mensaje);
    }

    public async Task EnviarSmsAsync(string telefono, string mensaje, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando SMS a {Telefono}", telefono);
        await _smsSender.SendAsync(telefono, mensaje);
    }
}