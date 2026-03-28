using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychoCitas.Infrastructure.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PsychoCitas.Infrastructure.Services.Notifications;

public class TwilioSmsSender
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioSmsSender> _logger;

    public TwilioSmsSender(
        IOptions<TwilioOptions> options,
        ILogger<TwilioSmsSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string phone, string messageBody)
    {
        if (string.IsNullOrWhiteSpace(_options.AccountSid) || string.IsNullOrWhiteSpace(_options.AuthToken))
            throw new InvalidOperationException("Twilio no está configurado correctamente.");

        if (string.IsNullOrWhiteSpace(_options.SmsFrom))
            throw new InvalidOperationException("Twilio:SmsFrom no está configurado.");

        TwilioClient.Init(_options.AccountSid, _options.AuthToken);

        var to = PhoneNumberNormalizer.NormalizePeru(phone, _options.DefaultCountryCode);

        var message = await MessageResource.CreateAsync(
            to: new Twilio.Types.PhoneNumber(to),
            from: new Twilio.Types.PhoneNumber(_options.SmsFrom),
            body: messageBody
        );

        if (message.ErrorCode is not null)
        {
            _logger.LogError("Twilio SMS error {ErrorCode}: {ErrorMessage}", message.ErrorCode, message.ErrorMessage);
            throw new InvalidOperationException($"No se pudo enviar el SMS: {message.ErrorMessage}");
        }
    }
}