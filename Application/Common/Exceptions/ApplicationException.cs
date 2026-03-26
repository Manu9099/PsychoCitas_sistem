namespace PsychoCitas.Application.Common.Exceptions;

public class ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    : Exception("Errores de validación.")
{
    public IDictionary<string, string[]> Errors { get; } = failures
        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
        .ToDictionary(g => g.Key, g => g.ToArray());
}
