using Microsoft.Extensions.Logging;
using PsychoCitas.Application.Common.Interfaces;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly ResendEmailSender _emailSender;
    private readonly TwilioSmsSender _smsSender;
    private readonly TwilioWhatsAppSender _whatsAppSender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ResendEmailSender emailSender,
        TwilioSmsSender smsSender,
        TwilioWhatsAppSender whatsAppSender,
        ILogger<NotificationService> logger)
    {
        _emailSender = emailSender;
        _smsSender = smsSender;
        _whatsAppSender = whatsAppSender;
        _logger = logger;
    }

    public async Task EnviarEmailAsync(string destino, string asunto, string mensaje, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando email a {Destino}", destino);
        await _emailSender.SendAsync(destino, asunto, mensaje, ct);
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