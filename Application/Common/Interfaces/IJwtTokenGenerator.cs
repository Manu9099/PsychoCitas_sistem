namespace PsychoCitas.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) Generate(Guid userId, string nombreUsuario, string email, string rol);
}