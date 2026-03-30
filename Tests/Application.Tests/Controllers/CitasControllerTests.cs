using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PsychoCitas.API.Controllers;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Application.Features.Agenda.Queries;
using PsychoCitas.Application.Features.Citas.Commands;
using PsychoCitas.Application.Features.Citas.Queries;
using PsychoCitas.Domain.Enums;
using Xunit;

namespace PsychoCitas.Application.Tests.Controllers;

public class CitasControllerTests
{
    [Fact]
    public async Task AgendaHoy_DeberiaRetornarOk()
    {
        var psicologoId = Guid.NewGuid();

        IEnumerable<CitaDto> response = new List<CitaDto>
        {
            new CitaDto(
                Guid.NewGuid(),              // Id
                Guid.NewGuid(),              // PacienteId
                "Luciana Torres",            // PacienteNombre
                "999999999",                 // PacienteTelefono
                psicologoId,                 // PsicologoId
                DateTime.UtcNow.AddHours(1), // FechaInicio
                DateTime.UtcNow.AddHours(2), // FechaFin
                TipoSesion.Individual,       // TipoSesion
                Modalidad.Presencial,        // Modalidad
                EstadoCita.Programada,       // Estado
                null,                        // LinkVideollamada
                1,                           // NumeroSesion
                true,                        // EsPrimeraVez
                "Primera cita",              // NotasPrevias
                null,                        // MotivoCancelacion
                EstadoPago.Pendiente,        // EstadoPago
                120m,                        // MontoCita
                0m,                          // MontoPagado
                120m,                        // SaldoPago
                null,                        // MetodoPago
                false                        // TieneNota
            )
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<GetAgendaHoyQuery>(q => q.PsicologoId == psicologoId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new CitasController(mediator.Object);

        var result = await controller.AgendaHoy(psicologoId, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Agendar_DeberiaRetornarCreatedAtAction()
    {
        var command = new AgendarCitaCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddMinutes(50),
            TipoSesion.Individual,
            Modalidad.Presencial,
            true,
            null,
            "Primera sesión");

        var response = new CitaResumenDto(
            Guid.NewGuid(),
            command.FechaInicio,
            command.FechaFin,
            "Luciana Torres",
            EstadoCita.Programada,
            command.Modalidad,
            command.TipoSesion,
            command.EsPrimeraVez);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new CitasController(mediator.Object);

        var result = await controller.Agendar(command, CancellationToken.None);

        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(CitasController.GetById));
        created.RouteValues.Should().NotBeNull();
        created.RouteValues!["id"].Should().Be(response.Id);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetById_DeberiaRetornarOk()
    {
        var citaId = Guid.NewGuid();

        var response = new CitaDto(
            citaId,
            Guid.NewGuid(),
            "Luciana Torres",
            "999999999",
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddMinutes(50),
            TipoSesion.Individual,
            Modalidad.Presencial,
            EstadoCita.Programada,
            null,
            1,
            true,
            "Notas",
            null,
            EstadoPago.Pendiente,
            120m,
            0m,
            120m,
            null,
            false);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<GetCitaByIdQuery>(q => q.CitaId == citaId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new CitasController(mediator.Object);

        var result = await controller.GetById(citaId, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Cancelar_DeberiaRetornarNoContent()
    {
        var citaId = Guid.NewGuid();
        var request = new CancelarRequest("Paciente no asistirá");

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<CancelarCitaCommand>(c => c.CitaId == citaId && c.Motivo == request.Motivo),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = new CitasController(mediator.Object);

        var result = await controller.Cancelar(citaId, request, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Completar_DeberiaRetornarNoContent()
    {
        var citaId = Guid.NewGuid();

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<CompletarCitaCommand>(c => c.CitaId == citaId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = new CitasController(mediator.Object);

        var result = await controller.Completar(citaId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task NoAsistio_DeberiaRetornarNoContent()
    {
        var citaId = Guid.NewGuid();

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(
                It.Is<MarcarNoAsistioCommand>(c => c.CitaId == citaId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = new CitasController(mediator.Object);

        var result = await controller.NoAsistio(citaId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }
}