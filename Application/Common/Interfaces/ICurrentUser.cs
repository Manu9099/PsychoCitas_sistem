namespace PsychoCitas.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    string Rol { get; }
    bool EsPsicologo => Rol == "psicologo";
    bool EsAdmin => Rol == "admin";
}
