using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioWhatsAppSender
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _from;

    public TwilioWhatsAppSender(string accountSid, string authToken, string from)
    {
        _accountSid = accountSid;
        _authToken = authToken;
        _from = from;
    }

    public async Task SendTemplateAsync(string toPhone)
    {
        TwilioClient.Init(_accountSid, _authToken);

        var messageOptions = new CreateMessageOptions(
            new PhoneNumber($"whatsapp:{toPhone}"));

        messageOptions.From = new PhoneNumber(_from);
        messageOptions.ContentSid = "HX229f5a04fd0510ce1b071852155d3e75";
        messageOptions.ContentVariables = "{\"1\":\"409173\"}";

        var message = await MessageResource.CreateAsync(messageOptions);
        Console.WriteLine(message.Sid);
    }
}