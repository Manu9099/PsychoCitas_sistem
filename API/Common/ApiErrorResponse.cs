namespace PsychoCitas.API.Common;

public class ApiErrorResponse
{
    public string Title { get; set; } = "Error";
    public int Status { get; set; }
    public string? Detail { get; set; }
    public object? Errors { get; set; }
    public string? TraceId { get; set; }
}