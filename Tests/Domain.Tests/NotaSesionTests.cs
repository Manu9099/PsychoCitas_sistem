using FluentAssertions;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Exceptions;
using Xunit;

namespace PsychoCitas.Domain.Tests;

public class NotaSesionTests
{
    [Fact]
    public void Finalizar_DeberiaFallar_SiNoHayResumen()
    {
        var nota = NotaSesion.Crear(Guid.NewGuid(), Guid.NewGuid());

        var act = () => nota.Finalizar();

        act.Should().Throw<DomainException>()
            .WithMessage("*resumen*");
    }

    [Fact]
    public void Actualizar_DeberiaFallar_SiLaNotaYaFueFinalizada()
    {
        var nota = NotaSesion.Crear(Guid.NewGuid(), Guid.NewGuid());
        nota.Actualizar("Resumen", new List<string> { "Escucha activa" }, 6, 4, null, null, null, null);
        nota.Finalizar();

        var act = () => nota.Actualizar("Nuevo", null, 5, 3, null, null, null, null);

        act.Should().Throw<DomainException>()
            .WithMessage("*finalizada*");
    }
}