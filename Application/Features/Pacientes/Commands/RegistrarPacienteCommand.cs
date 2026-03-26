using FluentValidation;
using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pacientes.Commands;

public record RegistrarPacienteCommand(
    string Nombres, string Apellidos,
    string? Email, string? Telefono,
    string? Dni = null, DateOnly? FechaNacimiento = null,
    string? Genero = null, string? Ocupacion = null,
    string? ReferidoPor = null
) : IRequest<PacienteDto>;

public class RegistrarPacienteValidator : AbstractValidator<RegistrarPacienteCommand>
{
    public RegistrarPacienteValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null);
        RuleFor(x => x.Telefono).MaximumLength(20).When(x => x.Telefono is not null);
    }
}

public class RegistrarPacienteHandler(IUnitOfWork uow) : IRequestHandler<RegistrarPacienteCommand, PacienteDto>
{
    public async Task<PacienteDto> Handle(RegistrarPacienteCommand cmd, CancellationToken ct)
    {
        var paciente = Paciente.Create(cmd.Nombres, cmd.Apellidos, cmd.Email, cmd.Telefono,
            cmd.Dni, cmd.FechaNacimiento, cmd.Genero, cmd.Ocupacion, cmd.ReferidoPor);

        await uow.Pacientes.AddAsync(paciente, ct);
        await uow.SaveChangesAsync(ct);

        return new PacienteDto(paciente.Id, paciente.Nombres, paciente.Apellidos,
            paciente.NombreCompleto, paciente.Edad, paciente.Dni, paciente.Email,
            paciente.Telefono, paciente.Genero, paciente.Ocupacion, paciente.ReferidoPor,
            paciente.Activo, paciente.CreadoEn);
    }
}
