// Tests/Application.Tests/RegistrarPagoHandlerTests.cs

using System.Reflection;
using FluentAssertions;
using Moq;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;

namespace PsychoCitas.Application.Tests;

public class RegistrarPagoHandlerTests
{
    private sealed class CurrentUserStub : ICurrentUser
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = "admin@demo.com";
        public string Rol { get; init; } = "Admin";
        public bool IsAuthenticated { get; init; } = true;
    }

    [Fact]
    public async Task Handle_DeberiaFallar_SiLaCitaNoExiste()
    {
        var citaId = Guid.NewGuid();

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cita?)null);

        var pagos = new Mock<IPagoRepository>();

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var handler = new RegistrarPagoHandler(uow.Object, currentUser);

        var act = async () => await handler.Handle(
            new RegistrarPagoCommand(
                citaId,
                180m,
                180m,
                "Yape",
                "OP-001",
                "Pago completo"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaCrearPago_YRegistrarlo_SiNoExistePagoPrevio()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pagos = new Mock<IPagoRepository>();
        pagos.Setup(x => x.GetByCitaIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pago?)null);

        Pago? pagoAgregado = null;
        pagos.Setup(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()))
            .Callback<Pago, CancellationToken>((p, _) => pagoAgregado = p)
            .Returns(Task.CompletedTask);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new CurrentUserStub
        {
            UserId = userId,
            IsAuthenticated = true
        };

        var handler = new RegistrarPagoHandler(uow.Object, currentUser);

        var result = await handler.Handle(
            new RegistrarPagoCommand(
                citaId,
                180m,
                180m,
                "Yape",
                "OP-002",
                "Pago completo"),
            CancellationToken.None);

        pagoAgregado.Should().NotBeNull();
        pagoAgregado!.CitaId.Should().Be(citaId);
        pagoAgregado.PacienteId.Should().Be(cita.PacienteId);
        pagoAgregado.Monto.Should().Be(180m);
        pagoAgregado.MontoPagado.Should().Be(180m);
        pagoAgregado.Saldo.Should().Be(0m);
        pagoAgregado.MetodoPago.Should().Be("Yape");
        pagoAgregado.NumeroOperacion.Should().Be("OP-002");
        pagoAgregado.Notas.Should().Be("Pago completo");
        pagoAgregado.Estado.Should().Be(EstadoPago.Pagado);

        result.Id.Should().Be(pagoAgregado.Id);
        result.CitaId.Should().Be(citaId);
        result.PacienteId.Should().Be(cita.PacienteId);
        result.Monto.Should().Be(180m);
        result.MontoPagado.Should().Be(180m);
        result.Saldo.Should().Be(0m);
        result.MetodoPago.Should().Be("Yape");
        result.NumeroOperacion.Should().Be("OP-002");
        result.Notas.Should().Be("Pago completo");
        result.Estado.Should().Be(EstadoPago.Pagado);

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaRegistrarPago_SobrePagoExistente()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var pagoExistente = Pago.Crear(citaId, pacienteId, 200m, Guid.NewGuid());

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pagos = new Mock<IPagoRepository>();
        pagos.Setup(x => x.GetByCitaIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagoExistente);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var handler = new RegistrarPagoHandler(uow.Object, currentUser);

        var result = await handler.Handle(
            new RegistrarPagoCommand(
                citaId,
                200m,
                200m,
                "Transferencia",
                "TX-123",
                "Pago por app"),
            CancellationToken.None);

        pagoExistente.Monto.Should().Be(200m);
        pagoExistente.MontoPagado.Should().Be(200m);
        pagoExistente.Saldo.Should().Be(0m);
        pagoExistente.MetodoPago.Should().Be("Transferencia");
        pagoExistente.NumeroOperacion.Should().Be("TX-123");
        pagoExistente.Notas.Should().Be("Pago por app");
        pagoExistente.Estado.Should().Be(EstadoPago.Pagado);

        result.Id.Should().Be(pagoExistente.Id);
        result.Monto.Should().Be(200m);
        result.MontoPagado.Should().Be(200m);
        result.Saldo.Should().Be(0m);
        result.MetodoPago.Should().Be("Transferencia");
        result.NumeroOperacion.Should().Be("TX-123");
        result.Notas.Should().Be("Pago por app");
        result.Estado.Should().Be(EstadoPago.Pagado);

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaFallar_SiElPagoYaEstaExonerado()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var pagoExonerado = Pago.Crear(citaId, pacienteId, 200m, Guid.NewGuid());
        ForzarEstadoPago(pagoExonerado, EstadoPago.Exonerado);

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pagos = new Mock<IPagoRepository>();
        pagos.Setup(x => x.GetByCitaIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagoExonerado);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var handler = new RegistrarPagoHandler(uow.Object, currentUser);

        var act = async () => await handler.Handle(
            new RegistrarPagoCommand(
                citaId,
                200m,
                200m,
                "Efectivo",
                "EF-001",
                "No debería registrar"),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("La cita ya está exonerada.");

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static void ForzarEstadoPago(Pago pago, EstadoPago estado)
    {
        var prop = typeof(Pago).GetProperty("Estado", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop is not null)
        {
            prop.SetValue(pago, estado);
            return;
        }

        var field = typeof(Pago).GetField("<Estado>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field is not null)
        {
            field.SetValue(pago, estado);
            return;
        }

        throw new InvalidOperationException("No se pudo forzar el estado del pago para el test.");
    }
}