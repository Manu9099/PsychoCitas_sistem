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
  [Fact]
public async Task Handle_DeberiaRegistrarPaciente_YGuardarCambios()
{
    var pacientes = new Mock<IPacienteRepository>();

    pacientes.Setup(x => x.GetByDniAsync("87654321", It.IsAny<CancellationToken>()))
        .ReturnsAsync((Paciente?)null);

    pacientes.Setup(x => x.GetByEmailAsync("luis@demo.com", It.IsAny<CancellationToken>()))
        .ReturnsAsync((Paciente?)null);

    Paciente? agregado = null;

    pacientes.Setup(x => x.AddAsync(It.IsAny<Paciente>(), It.IsAny<CancellationToken>()))
        .Callback<Paciente, CancellationToken>((p, _) => agregado = p)
        .Returns(Task.CompletedTask);

    var uow = new Mock<IUnitOfWork>();
    uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);
    uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    var handler = new RegistrarPacienteHandler(uow.Object);

    var result = await handler.Handle(
        new RegistrarPacienteCommand(
            " Luis ",
            " Torres ",
            " LUIS@DEMO.COM ",
            " 988888888 ",
            "87654321"),
        CancellationToken.None);

    agregado.Should().NotBeNull();
    agregado!.Nombres.Should().Be("Luis");
    agregado.Apellidos.Should().Be("Torres");
    agregado.Email.Should().Be("luis@demo.com");
    agregado.Telefono.Should().Be("988888888");
    agregado.Dni.Should().Be("87654321");

    result.Email.Should().Be("luis@demo.com");
    result.Nombres.Should().Be("Luis");
    result.Apellidos.Should().Be("Torres");

    pacientes.Verify(x => x.AddAsync(It.IsAny<Paciente>(), It.IsAny<CancellationToken>()), Times.Once);
    uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}


}