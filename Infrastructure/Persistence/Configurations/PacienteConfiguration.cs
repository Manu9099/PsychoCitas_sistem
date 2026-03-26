using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("pacientes");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Nombres).HasColumnName("nombres").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsRequired();
        builder.Property(p => p.FechaNacimiento).HasColumnName("fecha_nacimiento");
        builder.Property(p => p.Genero).HasColumnName("genero").HasMaxLength(30);
        builder.Property(p => p.Dni).HasColumnName("dni").HasMaxLength(20);
        builder.Property(p => p.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(p => p.Telefono).HasColumnName("telefono").HasMaxLength(20);
        builder.Property(p => p.TelefonoEmergencia).HasColumnName("telefono_emergencia").HasMaxLength(20);
        builder.Property(p => p.ContactoEmergencia).HasColumnName("contacto_emergencia").HasMaxLength(200);
        builder.Property(p => p.Ocupacion).HasColumnName("ocupacion").HasMaxLength(100);
        builder.Property(p => p.EstadoCivil).HasColumnName("estado_civil").HasMaxLength(50);
        builder.Property(p => p.Direccion).HasColumnName("direccion");
        builder.Property(p => p.ReferidoPor).HasColumnName("referido_por").HasMaxLength(200);
        builder.Property(p => p.Activo).HasColumnName("activo");
        builder.Property(p => p.CreadoEn).HasColumnName("creado_en");
        builder.Property(p => p.ActualizadoEn).HasColumnName("actualizado_en");

        builder.HasIndex(p => p.Dni).IsUnique().HasFilter("dni IS NOT NULL");
        builder.HasIndex(p => p.Email).HasFilter("email IS NOT NULL");
        builder.HasIndex(p => new { p.Apellidos, p.Nombres });

        builder.HasOne(p => p.HistoriaClinica)
            .WithOne(h => h.Paciente)
            .HasForeignKey<HistoriaClinica>(h => h.PacienteId);

        builder.Ignore(p => p.DomainEvents);
        builder.Ignore(p => p.NombreCompleto);
        builder.Ignore(p => p.Edad);
    }
}
