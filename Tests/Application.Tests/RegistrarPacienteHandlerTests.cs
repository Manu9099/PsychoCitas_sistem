using FluentAssertions;
using Moq;
using PsychoCitas.Application.Features.Pacientes.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;

namespace PsychoCitas.Application.Tests;

public class RegistrarPacienteHandlerTests
{
    [Fact]
    public async Task Handle_DeberiaFallar_SiElDniYaExiste()
    {
        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByDniAsync("12345678", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Paciente.Create("Ana", "Perez", "ana@demo.com", "999999999", "12345678"));

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);

        var handler = new RegistrarPacienteHandler(uow.Object);

        var act = async () => await handler.Handle(
            new RegistrarPacienteCommand("Luis", "Torres", "luis@demo.com", "988888888", "12345678"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*DNI*");
    }

    [Fact]
    public async Task Handle_DeberiaFallar_SiElEmailYaExiste()
    {
        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByEmailAsync("ana@demo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Paciente.Create("Ana", "Perez", "ana@demo.com", "999999999", "12345678"));

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);

        var handler = new RegistrarPacienteHandler(uow.Object);

        var act = async () => await handler.Handle(
            new RegistrarPacienteCommand("Luis", "Torres", "ana@demo.com", "988888888", "87654321"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*email*");
    }
}