namespace PsychoCitas.Infrastructure.Options;

public class LocalStorageOptions
{
    public const string SectionName = "LocalStorage";
    public string BasePath { get; set; } = "storage";
    public string PublicBaseUrl { get; set; } = "";
}