using FluentValidation;
using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pacientes.Commands;

public record RegistrarPacienteCommand(
    string Nombres,
    string Apellidos,
    string? Email,
    string? Telefono,
    string? Dni = null,
    DateOnly? FechaNacimiento = null,
    string? Genero = null,
    string? Ocupacion = null,
    string? ReferidoPor = null
) : IRequest<PacienteDto>;

public class RegistrarPacienteValidator : AbstractValidator<RegistrarPacienteCommand>
{
    public RegistrarPacienteValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Telefono).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Telefono));
        RuleFor(x => x.Dni).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Dni));
    }
}

public class RegistrarPacienteHandler(IUnitOfWork uow) : IRequestHandler<RegistrarPacienteCommand, PacienteDto>
{
    public async Task<PacienteDto> Handle(RegistrarPacienteCommand cmd, CancellationToken ct)
    {
        var dni = string.IsNullOrWhiteSpace(cmd.Dni) ? null : cmd.Dni.Trim();
        var email = string.IsNullOrWhiteSpace(cmd.Email) ? null : cmd.Email.Trim().ToLowerInvariant();

        if (dni is not null)
        {
            var existentePorDni = await uow.Pacientes.GetByDniAsync(dni, ct);
            if (existentePorDni is not null)
                throw new ConflictException($"Ya existe un paciente registrado con DNI '{dni}'.");
        }

        if (email is not null)
        {
            var existentePorEmail = await uow.Pacientes.GetByEmailAsync(email, ct);
            if (existentePorEmail is not null)
                throw new ConflictException($"Ya existe un paciente registrado con email '{email}'.");
        }

        var paciente = Paciente.Create(
            cmd.Nombres,
            cmd.Apellidos,
            email,
            cmd.Telefono,
            dni,
            cmd.FechaNacimiento,
            cmd.Genero,
            cmd.Ocupacion,
            cmd.ReferidoPor
        );

        await uow.Pacientes.AddAsync(paciente, ct);
        await uow.SaveChangesAsync(ct);

        return new PacienteDto(
            paciente.Id,
            paciente.Nombres,
            paciente.Apellidos,
            paciente.NombreCompleto,
            paciente.Edad,
            paciente.Dni,
            paciente.Email,
            paciente.Telefono,
            paciente.Genero,
            paciente.Ocupacion,
            paciente.ReferidoPor,
            paciente.Activo,
            paciente.CreadoEn
        );
    }
}