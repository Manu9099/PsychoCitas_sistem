using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var usuario = await uow.Usuarios.GetByEmailAsync(request.Email, ct);

        if (usuario is null || !usuario.Activo || !passwordHasher.Verify(usuario.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var (token, expiresAtUtc) = jwtTokenGenerator.Generate(
            usuario.Id,
            usuario.NombreUsuario,
            usuario.Email,
            usuario.Rol
        );

        return new LoginResponseDto(
            token,
            expiresAtUtc,
            usuario.Id,
            usuario.Email,
            usuario.Rol,
            usuario.NombreUsuario
        );
    }
}