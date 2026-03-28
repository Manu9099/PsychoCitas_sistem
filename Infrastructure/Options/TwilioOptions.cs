namespace PsychoCitas.Infrastructure.Options;

public class TwilioOptions
{
    public const string SectionName = "Twilio";

    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string SmsFrom { get; set; } = string.Empty;
    public string WhatsAppFrom { get; set; } = "whatsapp:+14155238886";
    public string DefaultCountryCode { get; set; } = "+51";
}