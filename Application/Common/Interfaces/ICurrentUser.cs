namespace PsychoCitas.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    string Rol { get; }
    bool IsAuthenticated { get; }

    bool EsPsicologo => string.Equals(Rol, "Psicologo", StringComparison.OrdinalIgnoreCase);
    bool EsAdmin => string.Equals(Rol, "Admin", StringComparison.OrdinalIgnoreCase);
}