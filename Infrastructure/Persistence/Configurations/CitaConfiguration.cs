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

        builder.Property(c => c.PacienteId).HasColumnName("paciente_id").IsRequired();
        builder.Property(c => c.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        builder.Property(c => c.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        builder.Property(c => c.FechaFin).HasColumnName("fecha_fin").IsRequired();

        builder.Property(c => c.TipoSesion)
            .HasColumnName("tipo_sesion")
            .HasConversion(v => v.ToString().ToLower(), v => Enum.Parse<TipoSesion>(v, true));

        builder.Property(c => c.Modalidad)
            .HasColumnName("modalidad")
            .HasConversion(v => v.ToString().ToLower(), v => Enum.Parse<Modalidad>(v, true));

        builder.Property(c => c.Estado)
            .HasColumnName("estado")
            .HasConversion(v => v.ToString().ToLower(), v => Enum.Parse<EstadoCita>(v, true));

        builder.Property(c => c.LinkVideollamada).HasColumnName("link_videollamada").HasMaxLength(500);
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

        // Relaciones
        builder.HasOne(c => c.Paciente)
            .WithMany(p => p.Citas)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Nota)
            .WithOne(n => n.Cita)
            .HasForeignKey<NotaSesion>(n => n.CitaId);

        builder.HasOne(c => c.Pago)
            .WithOne(p => p.Cita)
            .HasForeignKey<Pago>(p => p.CitaId);

        // Ignorar domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
