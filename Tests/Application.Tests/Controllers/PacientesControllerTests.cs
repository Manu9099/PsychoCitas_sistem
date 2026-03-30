

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PsychoCitas.API.Controllers;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Application.Features.Pacientes.Commands;
using PsychoCitas.Application.Features.Pacientes.Queries;
using Xunit;

namespace PsychoCitas.Application.Tests.Controllers;

public class PacientesControllerTests
{
    [Fact]
    public async Task GetDetalle_DeberiaRetornarOk()
    {
        var pacienteId = Guid.NewGuid();

        var response = new PacienteDetalleDto(
            pacienteId,
            "Luciana",
            "Torres",
            "Luciana Torres",
            22,
            "12345678",
            "luciana@mail.com",
            "999999999",
            null,
            null,
            "F",
            "Psicología",
            null,
            null,
            "Instagram",
            true,
            null,
            3,
            0,
            DateTime.UtcNow.AddDays(-3),
            0m);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<GetPacienteDetalleQuery>(q => q.PacienteId == pacienteId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PacientesController(mediator.Object);

        var result = await controller.GetDetalle(pacienteId, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Buscar_DeberiaRetornarOk()
    {
        const string termino = "luci";

        IEnumerable<PacienteDto> response = new List<PacienteDto>
        {
            new PacienteDto(
                Guid.NewGuid(),
                "Luciana",
                "Torres",
                "Luciana Torres",
                22,
                "12345678",
                "luciana@mail.com",
                "999999999",
                "F",
                "Psicología",
                "Instagram",
                true,
                DateTime.UtcNow)
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<BuscarPacientesQuery>(q => q.Termino == termino),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PacientesController(mediator.Object);

        var result = await controller.Buscar(termino, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Registrar_DeberiaRetornarCreatedAtAction()
    {
        var command = new RegistrarPacienteCommand(
            "Luciana",
            "Torres",
            "luciana@mail.com",
            "999999999",
            "12345678");

        var response = new PacienteDto(
            Guid.NewGuid(),
            "Luciana",
            "Torres",
            "Luciana Torres",
            22,
            "12345678",
            "luciana@mail.com",
            "999999999",
            null,
            null,
            null,
            true,
            DateTime.UtcNow);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new PacientesController(mediator.Object);

        var result = await controller.Registrar(command, CancellationToken.None);

        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(PacientesController.GetDetalle));
        created.RouteValues.Should().NotBeNull();
        created.RouteValues!["id"].Should().Be(response.Id);
        created.Value.Should().BeEquivalentTo(response);
    }
}