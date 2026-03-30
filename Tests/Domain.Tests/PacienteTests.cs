using FluentAssertions;
using PsychoCitas.Domain.Entities;
using Xunit;

namespace PsychoCitas.Domain.Tests;

public class PacienteTests
{
    [Fact]
    public void Create_DeberiaNormalizarCampos()
    {
        var fechaNacimiento = new DateOnly(2000, 1, 15);

        var paciente = Paciente.Create(
            "  Luciana  ",
            "  Torres  ",
            "  LUCIANA@MAIL.COM  ",
            " 999999999 ",
            " 12345678 ",
            fechaNacimiento);

        paciente.Nombres.Should().Be("Luciana");
        paciente.Apellidos.Should().Be("Torres");
        paciente.Email.Should().Be("luciana@mail.com");
        paciente.Telefono.Should().Be("999999999");
        paciente.Dni.Should().Be("12345678");
        paciente.Activo.Should().BeTrue();
        paciente.Edad.Should().NotBeNull();
    }

    [Fact]
    public void Actualizar_DeberiaActualizarYNormalizarDatos()
    {
        var paciente = Paciente.Create("Ana", "Perez", "ana@demo.com", "999999999");

        paciente.Actualizar(
            " Ana ",
            " Gomez ",
            " ANA2@DEMO.COM ",
            " 988888888 ",
            "Psicóloga",
            "Soltera",
            "Lima");

        paciente.Nombres.Should().Be("Ana");
        paciente.Apellidos.Should().Be("Gomez");
        paciente.Email.Should().Be("ana2@demo.com");
        paciente.Telefono.Should().Be("988888888");
        paciente.Ocupacion.Should().Be("Psicóloga");
        paciente.EstadoCivil.Should().Be("Soltera");
        paciente.Direccion.Should().Be("Lima");
    }

    [Fact]
    public void Desactivar_DeberiaMarcarPacienteComoInactivo()
    {
        var paciente = Paciente.Create("Ana", "Perez", "ana@demo.com", "999999999");

        paciente.Desactivar();

        paciente.Activo.Should().BeFalse();
    }
}