// Tests/Application.Tests/ConfirmarPagoIntentoHandlerTests.cs

using System.Reflection;
using FluentAssertions;
using Moq;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;

namespace PsychoCitas.Application.Tests;

public class ConfirmarPagoIntentoHandlerTests
{
    [Fact]
    public async Task Handle_DeberiaFallar_SiNoExisteExternalReference()
    {
        var intentos = new Mock<IIntentoPagoRepository>();
        intentos.Setup(x => x.GetByExternalReferenceAsync("ext-ref-404", It.IsAny<CancellationToken>()))
            .ReturnsAsync((IntentoPago?)null);

        var eventos = new Mock<IEventoPagoRepository>();

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.IntentosPago).Returns(intentos.Object);
        uow.SetupGet(x => x.EventosPago).Returns(eventos.Object);

        var handler = new ConfirmarPagoIntentoHandler(uow.Object);

        var act = async () => await handler.Handle(
            new ConfirmarPagoIntentoCommand(
                "ext-ref-404",
                150m,
                "Yape",
                "OP-001",
                "{\"status\":\"paid\"}"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        eventos.Verify(x => x.AddAsync(It.IsAny<EventoPago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaFallar_SiIntentoNoTienePagoAsociado()
    {
        var intento = IntentoPago.Crear(
            Guid.NewGuid(),
            ProveedorPago.MockCheckout,
            150m,
            "PEN",
            "ext-ref-001",
            "https://checkout.mock/pago-1",
            DateTime.UtcNow.AddMinutes(30));

        var intentos = new Mock<IIntentoPagoRepository>();
        intentos.Setup(x => x.GetByExternalReferenceAsync("ext-ref-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(intento);

        var eventos = new Mock<IEventoPagoRepository>();

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.IntentosPago).Returns(intentos.Object);
        uow.SetupGet(x => x.EventosPago).Returns(eventos.Object);

        var handler = new ConfirmarPagoIntentoHandler(uow.Object);

        var act = async () => await handler.Handle(
            new ConfirmarPagoIntentoCommand(
                "ext-ref-001",
                150m,
                "Yape",
                "OP-002",
                "{\"status\":\"paid\"}"),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Intento sin pago asociado.");

        eventos.Verify(x => x.AddAsync(It.IsAny<EventoPago>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaConfirmarIntentoYRegistrarPago()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();

        var pago = Pago.Crear(citaId, pacienteId, 180m, Guid.NewGuid());

        var intento = IntentoPago.Crear(
            pago.Id,
            ProveedorPago.MockCheckout,
            180m,
            "PEN",
            "ext-ref-002",
            "https://checkout.mock/pago-2",
            DateTime.UtcNow.AddMinutes(30));

        VincularPagoAlIntento(intento, pago);

        var intentos = new Mock<IIntentoPagoRepository>();
        intentos.Setup(x => x.GetByExternalReferenceAsync("ext-ref-002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(intento);

        EventoPago? eventoAgregado = null;
        var eventos = new Mock<IEventoPagoRepository>();
        eventos.Setup(x => x.AddAsync(It.IsAny<EventoPago>(), It.IsAny<CancellationToken>()))
            .Callback<EventoPago, CancellationToken>((e, _) => eventoAgregado = e)
            .Returns(Task.CompletedTask);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.IntentosPago).Returns(intentos.Object);
        uow.SetupGet(x => x.EventosPago).Returns(eventos.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ConfirmarPagoIntentoHandler(uow.Object);

        var result = await handler.Handle(
            new ConfirmarPagoIntentoCommand(
                "ext-ref-002",
                180m,
                "Yape",
                "OP-999",
                "{\"status\":\"paid\",\"provider\":\"mock\"}"),
            CancellationToken.None);

        eventoAgregado.Should().NotBeNull();
        eventoAgregado!.IntentoPagoId.Should().Be(intento.Id);
        eventoAgregado.Proveedor.Should().Be(ProveedorPago.MockCheckout);

        intento.Estado.Should().Be(EstadoIntentoPago.Exitoso);
       

        pago.MontoPagado.Should().Be(180m);
        pago.Saldo.Should().Be(0m);
        pago.MetodoPago.Should().Be("Yape");
        pago.NumeroOperacion.Should().Be("OP-999");
        pago.Estado.Should().Be(EstadoPago.Pagado);

        result.Id.Should().Be(pago.Id);
        result.CitaId.Should().Be(citaId);
        result.PacienteId.Should().Be(pacienteId);
        result.Monto.Should().Be(180m);
        result.MontoPagado.Should().Be(180m);
        result.Saldo.Should().Be(0m);
        result.MetodoPago.Should().Be("Yape");
        result.NumeroOperacion.Should().Be("OP-999");
        result.Estado.Should().Be(EstadoPago.Pagado);

        eventos.Verify(x => x.AddAsync(It.IsAny<EventoPago>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void VincularPagoAlIntento(IntentoPago intento, Pago pago)
    {
        var prop = typeof(IntentoPago).GetProperty("Pago", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop is not null)
        {
            prop.SetValue(intento, pago);
            return;
        }

        var field = typeof(IntentoPago).GetField("<Pago>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field is not null)
        {
            field.SetValue(intento, pago);
            return;
        }

        throw new InvalidOperationException("No se pudo vincular Pago al IntentoPago para el test.");
    }
}