// Tests/Application.Tests/Controllers/PagosControllerTests.cs

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PsychoCitas.API.Controllers;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Application.Features.Pagos.Commands;
using PsychoCitas.Application.Features.Pagos.Queries;
using PsychoCitas.Domain.Enums;
using Xunit;

namespace PsychoCitas.Application.Tests.Controllers;

public class PagosControllerTests
{
    [Fact]
    public async Task Registrar_DeberiaRetornarOk()
    {
        var command = new RegistrarPagoCommand(
            Guid.NewGuid(),
            180m,
            180m,
            "Yape",
            "OP-001",
            "Pago completo");

        var response = new PagoDto(
            Guid.NewGuid(),
            command.CitaId,
            Guid.NewGuid(),
            180m,
            180m,
            0m,
            EstadoPago.Pagado,
            "Yape",
            "OP-001",
            "Pago completo",
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PagosController(mediator.Object);

        var result = await controller.Registrar(command, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Exonerar_DeberiaRetornarOk()
    {
        var command = new ExonerarPagoCommand(
            Guid.NewGuid(),
            180m,
            "Exoneración social");

        var response = new PagoDto(
            Guid.NewGuid(),
            command.CitaId,
            Guid.NewGuid(),
            180m,
            0m,
            180m,
            EstadoPago.Exonerado,
            null,
            null,
            "Exoneración social",
            null,
            null,
            DateTime.UtcNow);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PagosController(mediator.Object);

        var result = await controller.Exonerar(command, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetByCita_DeberiaRetornarOk()
    {
        var citaId = Guid.NewGuid();

        var response = new PagoDto(
            Guid.NewGuid(),
            citaId,
            Guid.NewGuid(),
            200m,
            50m,
            150m,
            EstadoPago.Pendiente,
            "Transferencia",
            "TX-100",
            "Pago parcial",
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<GetPagoByCitaIdQuery>(q => q.CitaId == citaId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PagosController(mediator.Object);

        var result = await controller.GetByCita(citaId, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetByPaciente_DeberiaRetornarOk()
    {
        var pacienteId = Guid.NewGuid();

List<PagoResumenPacienteDto> response = new()
{
    new PagoResumenPacienteDto(
        Guid.NewGuid(),
        Guid.NewGuid(),
        180m,
        180m,
        0m,
        EstadoPago.Pagado,
        "Yape",
        DateTime.UtcNow,
        DateTime.UtcNow.AddDays(-2))
};

var mediator = new Mock<IMediator>();
mediator.Setup(m => m.Send(
        It.Is<GetPagosByPacienteIdQuery>(q => q.PacienteId == pacienteId),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(response);

        var controller = new PagosController(mediator.Object);

        var result = await controller.GetByPaciente(pacienteId, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }
}