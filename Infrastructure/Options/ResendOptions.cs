namespace PsychoCitas.Infrastructure.Options;

public class ResendOptions
{
    public const string SectionName = "Resend";
    public string FromEmail { get; set; } = "onboarding@resend.dev";
    public string FromName { get; set; } = "PsychoCitas";
}