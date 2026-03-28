using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychoCitas.Infrastructure.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class TwilioWhatsAppSender
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioWhatsAppSender> _logger;

    public TwilioWhatsAppSender(
        IOptions<TwilioOptions> options,
        ILogger<TwilioWhatsAppSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string phone, string messageBody)
    {
        if (string.IsNullOrWhiteSpace(_options.AccountSid) || string.IsNullOrWhiteSpace(_options.AuthToken))
            throw new InvalidOperationException("Twilio no está configurado correctamente.");

        if (string.IsNullOrWhiteSpace(_options.WhatsAppFrom))
            throw new InvalidOperationException("Twilio:WhatsAppFrom no está configurado.");

        TwilioClient.Init(_options.AccountSid, _options.AuthToken);

        var to = PhoneNumberNormalizer.ToWhatsAppAddress(phone, _options.DefaultCountryCode);

        var message = await MessageResource.CreateAsync(
            to: new PhoneNumber(to),
            from: new PhoneNumber(_options.WhatsAppFrom),
            body: messageBody
        );

        if (message.ErrorCode is not null)
        {
            _logger.LogError("Twilio WhatsApp error {ErrorCode}: {ErrorMessage}", message.ErrorCode, message.ErrorMessage);
            throw new InvalidOperationException($"No se pudo enviar el WhatsApp: {message.ErrorMessage}");
        }
    }
}