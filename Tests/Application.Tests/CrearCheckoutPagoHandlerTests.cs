// Tests/Application.Tests/CrearCheckoutPagoHandlerTests.cs

using FluentAssertions;
using Moq;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Interfaces;
using Xunit;


// Ajusta este using al nombre real de tu DTO de respuesta del gateway si cambia:
using PsychoCitas.Application.DTOs;

namespace PsychoCitas.Application.Tests;

public class CrearCheckoutPagoHandlerTests
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

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var paymentGateway = new Mock<IPaymentGateway>();

        var handler = new CrearCheckoutPagoHandler(
            uow.Object,
            currentUser,
            paymentGateway.Object);

        var act = async () => await handler.Handle(
            new CrearCheckoutPagoCommand(citaId, 120m, "PEN"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        paymentGateway.Verify(
            x => x.CrearCheckoutAsync(It.IsAny<PaymentCheckoutRequestDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaFallar_SiPacienteNoTieneEmail()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(3),
            DateTime.UtcNow.AddHours(4),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var paciente = Paciente.Create(
            "Luciana",
            "Torres",
            null,
            "999999999");

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByIdAsync(cita.PacienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paciente);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var paymentGateway = new Mock<IPaymentGateway>();

        var handler = new CrearCheckoutPagoHandler(
            uow.Object,
            currentUser,
            paymentGateway.Object);

        var act = async () => await handler.Handle(
            new CrearCheckoutPagoCommand(citaId, 150m, "PEN"),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("El paciente necesita email para checkout automático.");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        paymentGateway.Verify(
            x => x.CrearCheckoutAsync(It.IsAny<PaymentCheckoutRequestDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaCrearPagoEIntento_SiNoExistePagoPrevio()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(3),
            DateTime.UtcNow.AddHours(4),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var paciente = Paciente.Create(
            "Luciana",
            "Torres",
            "luciana@mail.com",
            "999999999");

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByIdAsync(cita.PacienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paciente);

        var pagos = new Mock<IPagoRepository>();
        pagos.Setup(x => x.GetByCitaIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pago?)null);

        Pago? pagoAgregado = null;
        pagos.Setup(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()))
            .Callback<Pago, CancellationToken>((p, _) => pagoAgregado = p)
            .Returns(Task.CompletedTask);

        var intentosPago = new Mock<IIntentoPagoRepository>();
        IntentoPago? intentoAgregado = null;
        intentosPago.Setup(x => x.AddAsync(It.IsAny<IntentoPago>(), It.IsAny<CancellationToken>()))
            .Callback<IntentoPago, CancellationToken>((i, _) => intentoAgregado = i)
            .Returns(Task.CompletedTask);

        PaymentCheckoutRequestDto? requestEnviado = null;

        var expiraEn = DateTime.UtcNow.AddMinutes(30);

        var paymentGateway = new Mock<IPaymentGateway>();
        paymentGateway.Setup(x => x.CrearCheckoutAsync(
                It.IsAny<PaymentCheckoutRequestDto>(),
                It.IsAny<CancellationToken>()))
            .Callback<PaymentCheckoutRequestDto, CancellationToken>((req, _) => requestEnviado = req)

      .ReturnsAsync(new PaymentCheckoutDto(
    CheckoutUrl: "https://checkout.mock/pago-1",
    ExternalReference: "ext-ref-001",
    ExpiraEn: expiraEn));

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);
        uow.SetupGet(x => x.IntentosPago).Returns(intentosPago.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new CurrentUserStub
        {
            UserId = userId,
            IsAuthenticated = true
        };

        var handler = new CrearCheckoutPagoHandler(
            uow.Object,
            currentUser,
            paymentGateway.Object);

        var result = await handler.Handle(
            new CrearCheckoutPagoCommand(citaId, 180m, "PEN"),
            CancellationToken.None);

        pagoAgregado.Should().NotBeNull();
        pagoAgregado!.CitaId.Should().Be(citaId);
        pagoAgregado.PacienteId.Should().Be(cita.PacienteId);
        pagoAgregado.Monto.Should().Be(180m);
        pagoAgregado.Estado.Should().Be(EstadoPago.Pendiente);

        requestEnviado.Should().NotBeNull();
        requestEnviado!.CitaId.Should().Be(cita.Id);
        requestEnviado.PacienteId.Should().Be(paciente.Id);
        requestEnviado.Monto.Should().Be(180m);
        requestEnviado.Moneda.Should().Be("PEN");
        requestEnviado.EmailPaciente.Should().Be("luciana@mail.com");

        intentoAgregado.Should().NotBeNull();
        intentoAgregado!.PagoId.Should().Be(pagoAgregado.Id);
        intentoAgregado.Proveedor.Should().Be(ProveedorPago.MockCheckout);
        intentoAgregado.Monto.Should().Be(180m);
        intentoAgregado.Moneda.Should().Be("PEN");
        intentoAgregado.ExternalReference.Should().Be("ext-ref-001");
        intentoAgregado.CheckoutUrl.Should().Be("https://checkout.mock/pago-1");
        intentoAgregado.Estado.Should().Be(EstadoIntentoPago.Pendiente);

        result.PagoId.Should().Be(pagoAgregado.Id);
        result.IntentoPagoId.Should().Be(intentoAgregado.Id);
        result.CheckoutUrl.Should().Be("https://checkout.mock/pago-1");
        result.ExternalReference.Should().Be("ext-ref-001");
        result.ExpiraEn.Should().Be(expiraEn);
        result.Monto.Should().Be(180m);
        result.Moneda.Should().Be("PEN");

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Once);
        intentosPago.Verify(x => x.AddAsync(It.IsAny<IntentoPago>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_DeberiaReutilizarPagoExistente_YCrearNuevoIntento()
    {
        var citaId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var psicologoId = Guid.NewGuid();

        var cita = Cita.Crear(
            pacienteId,
            psicologoId,
            DateTime.UtcNow.AddHours(3),
            DateTime.UtcNow.AddHours(4),
            TipoSesion.Individual,
            Modalidad.Presencial);

        var paciente = Paciente.Create(
            "Luciana",
            "Torres",
            "luciana@mail.com",
            "999999999");

        var pagoExistente = Pago.Crear(citaId, pacienteId, 220m, Guid.NewGuid());

        var citas = new Mock<ICitaRepository>();
        citas.Setup(x => x.GetByIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var pacientes = new Mock<IPacienteRepository>();
        pacientes.Setup(x => x.GetByIdAsync(cita.PacienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paciente);

        var pagos = new Mock<IPagoRepository>();
        pagos.Setup(x => x.GetByCitaIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagoExistente);

        var intentosPago = new Mock<IIntentoPagoRepository>();
        IntentoPago? intentoAgregado = null;
        intentosPago.Setup(x => x.AddAsync(It.IsAny<IntentoPago>(), It.IsAny<CancellationToken>()))
            .Callback<IntentoPago, CancellationToken>((i, _) => intentoAgregado = i)
            .Returns(Task.CompletedTask);

        var expiraEn = DateTime.UtcNow.AddMinutes(45);

        var paymentGateway = new Mock<IPaymentGateway>();
        paymentGateway.Setup(x => x.CrearCheckoutAsync(
                It.IsAny<PaymentCheckoutRequestDto>(),
                It.IsAny<CancellationToken>()))
// 
  .ReturnsAsync(new PaymentCheckoutDto(
    CheckoutUrl: "https://checkout.mock/pago-2",
    ExternalReference: "ext-ref-002",
    ExpiraEn: expiraEn));
    
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Citas).Returns(citas.Object);
        uow.SetupGet(x => x.Pacientes).Returns(pacientes.Object);
        uow.SetupGet(x => x.Pagos).Returns(pagos.Object);
        uow.SetupGet(x => x.IntentosPago).Returns(intentosPago.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new CurrentUserStub
        {
            UserId = Guid.NewGuid(),
            IsAuthenticated = true
        };

        var handler = new CrearCheckoutPagoHandler(
            uow.Object,
            currentUser,
            paymentGateway.Object);

        var result = await handler.Handle(
            new CrearCheckoutPagoCommand(citaId, 220m, "PEN"),
            CancellationToken.None);

        result.PagoId.Should().Be(pagoExistente.Id);

        intentoAgregado.Should().NotBeNull();
        intentoAgregado!.PagoId.Should().Be(pagoExistente.Id);
        intentoAgregado.Estado.Should().Be(EstadoIntentoPago.Pendiente);
        intentoAgregado.ExternalReference.Should().Be("ext-ref-002");
        intentoAgregado.CheckoutUrl.Should().Be("https://checkout.mock/pago-2");

        pagos.Verify(x => x.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Never);
        intentosPago.Verify(x => x.AddAsync(It.IsAny<IntentoPago>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}