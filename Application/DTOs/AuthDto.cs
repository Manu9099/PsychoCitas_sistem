namespace PsychoCitas.Application.DTOs;

public record LoginResponseDto(
    string Token,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string Rol,
    string NombreUsuario
);