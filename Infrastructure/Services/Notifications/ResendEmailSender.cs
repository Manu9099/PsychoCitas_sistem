using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychoCitas.Infrastructure.Options;
using Resend;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class ResendEmailSender
{
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailSender> _logger;
    private readonly IResend _resend;

    public ResendEmailSender(
        IOptions<ResendOptions> options,
        ILogger<ResendEmailSender> logger,
        IResend resend)
    {
        _options = options.Value;
        _logger = logger;
        _resend = resend;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string plainTextContent,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.FromEmail))
            throw new InvalidOperationException("Resend:FromEmail no está configurado.");

        var message = new EmailMessage
        {
            From = $"{_options.FromName} <{_options.FromEmail}>",
            Subject = subject,
            TextBody = plainTextContent
        };

        message.To.Add(to);

        var response = await _resend.EmailSendAsync(message, ct);

        if (!response.Success)
        {
            var error = response.Exception?.Message ?? "Error desconocido al enviar con Resend.";
            _logger.LogError("Resend error: {Error}", error);
            throw new InvalidOperationException($"No se pudo enviar el correo: {error}");
        }

        _logger.LogInformation("Correo enviado con Resend. Id={Id}", response.Content);
    }
}