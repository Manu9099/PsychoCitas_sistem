using FluentAssertions;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using Xunit;

namespace PsychoCitas.Domain.Tests;

public class CitaTests
{
    [Fact]
    public void Crear_DeberiaFallar_SiLaFechaEstaEnElPasado()
    {
        var act = () => Cita.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-10),
            DateTime.UtcNow.AddMinutes(40),
            TipoSesion.Individual,
            Modalidad.Presencial);

        act.Should().Throw<DomainException>()
            .WithMessage("*pasado*");
    }

    [Fact]
    public void Crear_DeberiaFallar_SiEsVideollamadaYSinLink()
    {
        var act = () => Cita.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoSesion.Individual,
            Modalidad.Videollamada,
            linkVideollamada: null);

        act.Should().Throw<DomainException>()
            .WithMessage("*link*");
    }

    [Fact]
    public void Completar_DeberiaCambiarEstadoANumeroSesion()
    {
        var cita = Cita.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoSesion.Individual,
            Modalidad.Presencial);

        cita.Completar(3);

        cita.Estado.Should().Be(EstadoCita.Completada);
        cita.NumeroSesion.Should().Be(3);
    }
}