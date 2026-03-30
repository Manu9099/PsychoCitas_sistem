// Tests/Application.Tests/ExonerarPagoHandlerTests.cs

using FluentAssertions;
using Moq;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;

namespace PsychoCitas.Application.Tests;

public class ExonerarPagoHandlerTests
{
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

        var handler = new ExonerarPagoHandler(uow.Object);

        var act = async () => await handler.Handle(
            new ExonerarPagoCommand(
                citaId,
                180m,
                "Exoneración social"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaCrearPagoYExonerarlo_SiNoExistePagoPrevio()
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

        var handler = new ExonerarPagoHandler(uow.Object);

        await handler.Handle(
            new ExonerarPagoCommand(
                citaId,
                180m,
                "Exoneración institucional"),
            CancellationToken.None);

        pagoAgregado.Should().NotBeNull();
        pagoAgregado!.CitaId.Should().Be(citaId);
        pagoAgregado.PacienteId.Should().Be(cita.PacienteId);
        pagoAgregado.Monto.Should().Be(180m);
        pagoAgregado.MontoPagado.Should().Be(0m);
        pagoAgregado.Saldo.Should().Be(180m);
        pagoAgregado.Estado.Should().Be(EstadoPago.Exonerado);
        pagoAgregado.Notas.Should().Be("Exoneración institucional");

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaExonerarPagoExistente()
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

        var pagoExistente = Pago.Crear(citaId, pacienteId, 220m, Guid.NewGuid());

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

        var handler = new ExonerarPagoHandler(uow.Object);

        await handler.Handle(
            new ExonerarPagoCommand(
                citaId,
                220m,
                "Beca / apoyo económico"),
            CancellationToken.None);

        pagoExistente.Monto.Should().Be(220m);
        pagoExistente.MontoPagado.Should().Be(0m);
        pagoExistente.Saldo.Should().Be(220m);
        pagoExistente.Estado.Should().Be(EstadoPago.Exonerado);
        pagoExistente.Notas.Should().Be("Beca / apoyo económico");

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}