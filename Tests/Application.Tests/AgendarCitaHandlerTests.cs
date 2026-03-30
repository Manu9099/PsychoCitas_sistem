using FluentAssertions;
using Moq;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.Features.Citas.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;

namespace PsychoCitas.Application.Tests;

public class AgendarCitaHandlerTests
{
    [Fact]
    public async Task Handle_DeberiaFallar_SiHaySolapamiento()
    {
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();
        var inicio = DateTime.UtcNow.AddHours(2);
        var fin = inicio.AddMinutes(50);

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.ExisteSolapamientoAsync(psicologoId, inicio, fin, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);

        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(x => x.UserId).Returns(psicologoId);
        currentUser.SetupGet(x => x.Rol).Returns("Psicologo");

        var notifications = new Mock<INotificationService>();

        var handler = new AgendarCitaHandler(uow.Object, currentUser.Object, notifications.Object);

        var act = async () => await handler.Handle(
            new AgendarCitaCommand(
                pacienteId,
                psicologoId,
                inicio,
                fin,
                TipoSesion.Individual,
                Modalidad.Presencial),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictoAgendaException>();
    }

[Fact]
public async Task Handle_DeberiaFallar_SiPsicologoIntentaAgendarParaOtro()
{
    var pacienteId = Guid.NewGuid();
    var psicologoLogueado = Guid.NewGuid();
    var otroPsicologo = Guid.NewGuid();
    var inicio = DateTime.UtcNow.AddHours(2);
    var fin = inicio.AddMinutes(50);

    var citas = new Mock<ICitaRepository>();
    citas.Setup(x => x.ExisteSolapamientoAsync(
            otroPsicologo,
            inicio,
            fin,
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var uow = new Mock<IUnitOfWork>();
    uow.SetupGet(x => x.Citas).Returns(citas.Object);

    var currentUser = new CurrentUserStub
    {
        UserId = psicologoLogueado,
        Rol = "Psicologo",
        Email = "psi@demo.com",
        IsAuthenticated = true
    };

    var notifications = new Mock<INotificationService>();

    var handler = new AgendarCitaHandler(uow.Object, currentUser, notifications.Object);

    var act = async () => await handler.Handle(
        new AgendarCitaCommand(
            pacienteId,
            otroPsicologo,
            inicio,
            fin,
            TipoSesion.Individual,
            Modalidad.Presencial),
        CancellationToken.None);

    await act.Should()
        .ThrowAsync<UnauthorizedAccessException>()
        .WithMessage("No puedes agendar citas para otro psicólogo.");
}

    [Fact]
    public async Task Handle_DeberiaCrearCita_YGuardarCambios()
    {
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();
        var inicio = DateTime.UtcNow.AddHours(2);
        var fin = inicio.AddMinutes(50);

        var paciente = Paciente.Create("Ana", "Perez", "ana@demo.com", "999999999");

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.ExisteSolapamientoAsync(psicologoId, inicio, fin, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Cita? citaAgregada = null;
        citas.Setup(x => x.AddAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()))
            .Callback<Cita, CancellationToken>((c, _) => citaAgregada = c)
            .Returns(Task.CompletedTask);

        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByIdAsync(pacienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paciente);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(x => x.UserId).Returns(psicologoId);
        currentUser.SetupGet(x => x.Rol).Returns("Psicologo");

        var notifications = new Mock<INotificationService>();
        notifications.Setup(x => x.EnviarEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AgendarCitaHandler(uow.Object, currentUser.Object, notifications.Object);

        var result = await handler.Handle(
            new AgendarCitaCommand(
                pacienteId,
                psicologoId,
                inicio,
                fin,
                TipoSesion.Individual,
                Modalidad.Presencial,
                false,
                null,
                "Primera entrevista"),
            CancellationToken.None);

        citaAgregada.Should().NotBeNull();
        citaAgregada!.PacienteId.Should().Be(pacienteId);
        citaAgregada.PsicologoId.Should().Be(psicologoId);
        citaAgregada.Estado.Should().Be(EstadoCita.Programada);
        citaAgregada.NotasPrevias.Should().Be("Primera entrevista");

        citas.Verify(x => x.AddAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
    }  

internal sealed class CurrentUserStub : ICurrentUser
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = "test@demo.com";
    public string Rol { get; init; } = "Psicologo";
    public bool IsAuthenticated { get; init; } = true;
}


}