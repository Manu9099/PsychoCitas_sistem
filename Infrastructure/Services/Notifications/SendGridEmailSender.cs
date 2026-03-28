using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychoCitas.Infrastructure.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class SendGridEmailSender
{
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string plainTextContent,
        string? htmlContent = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("SendGrid:ApiKey no está configurado.");

        if (string.IsNullOrWhiteSpace(_options.FromEmail))
            throw new InvalidOperationException("SendGrid:FromEmail no está configurado.");

        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("El destinatario es obligatorio.", nameof(to));

        var client = new SendGridClient(_options.ApiKey);

        var from = new EmailAddress(_options.FromEmail, _options.FromName);
        var recipient = new EmailAddress(to.Trim());

        var message = MailHelper.CreateSingleEmail(
            from,
            recipient,
            subject,
            plainTextContent,
            htmlContent ?? $"<p>{System.Net.WebUtility.HtmlEncode(plainTextContent)}</p>");

        var response = await client.SendEmailAsync(message, ct);

        if ((int)response.StatusCode >= 400)
        {
            var body = await response.Body.ReadAsStringAsync(ct);
            _logger.LogError("Error SendGrid {StatusCode}: {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"No se pudo enviar el correo. SendGrid respondió {(int)response.StatusCode}.");
        }
    }
}