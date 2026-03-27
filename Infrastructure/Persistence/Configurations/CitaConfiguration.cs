using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("citas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.PacienteId).HasColumnName("paciente_id");
        builder.Property(c => c.PsicologoId).HasColumnName("psicologo_id");
        builder.Property(c => c.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(c => c.FechaFin).HasColumnName("fecha_fin");
        builder.Property(c => c.LinkVideollamada).HasColumnName("link_videollamada");
        builder.Property(c => c.NumeroSesion).HasColumnName("numero_sesion");
        builder.Property(c => c.EsPrimeraVez).HasColumnName("es_primera_vez");
        builder.Property(c => c.NotasPrevias).HasColumnName("notas_previas");
        builder.Property(c => c.MotivoCancelacion).HasColumnName("motivo_cancelacion");
        builder.Property(c => c.CanceladoPor).HasColumnName("cancelado_por");
        builder.Property(c => c.CanceladoEn).HasColumnName("cancelado_en");
        builder.Property(c => c.RecurrenciaId).HasColumnName("recurrencia_id");
        builder.Property(c => c.CreadoPor).HasColumnName("creado_por");
        builder.Property(c => c.CreadoEn).HasColumnName("creado_en");
        builder.Property(c => c.ActualizadoEn).HasColumnName("actualizado_en");

       builder.Property(c => c.TipoSesion)
        .HasColumnName("tipo_sesion")
        .HasColumnType("tipo_sesion");

        builder.Property(c => c.Modalidad)
            .HasColumnName("modalidad")
            .HasColumnType("modalidad");

        builder.Property(c => c.Estado)
            .HasColumnName("estado")
            .HasColumnType("estado_cita");

        builder.HasOne(c => c.Paciente)
            .WithMany(p => p.Citas)
            .HasForeignKey(c => c.PacienteId)
            .HasPrincipalKey(p => p.Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Nota)
            .WithOne(n => n.Cita)
            .HasForeignKey<NotaSesion>(n => n.CitaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Pago)
            .WithOne(p => p.Cita)
            .HasForeignKey<Pago>(p => p.CitaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.DomainEvents);
        builder.Ignore(c => c.Duracion);
    }

    private static string ConvertEstadoToDb(EstadoCita estado)
    {
        return estado switch
        {
            EstadoCita.Programada => "programada",
            EstadoCita.Confirmada => "confirmada",
            EstadoCita.Completada => "completada",
            EstadoCita.Cancelada => "cancelada",
            EstadoCita.NoAsistio => "no_asistio",
            EstadoCita.Reagendada => "reagendada",
            _ => throw new InvalidOperationException("EstadoCita no válido.")
        };
    }

    private static EstadoCita ConvertEstadoFromDb(string estado)
    {
        return estado switch
        {
            "programada" => EstadoCita.Programada,
            "confirmada" => EstadoCita.Confirmada,
            "completada" => EstadoCita.Completada,
            "cancelada" => EstadoCita.Cancelada,
            "no_asistio" => EstadoCita.NoAsistio,
            "reagendada" => EstadoCita.Reagendada,
            _ => throw new InvalidOperationException($"Estado de cita no válido en BD: {estado}")
        };
    }
}