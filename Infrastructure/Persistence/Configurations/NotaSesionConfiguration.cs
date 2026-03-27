using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class NotaSesionConfiguration : IEntityTypeConfiguration<NotaSesion>
{
    public void Configure(EntityTypeBuilder<NotaSesion> builder)
    {
        builder.ToTable("notas_sesion");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id).HasColumnName("id");
        builder.Property(n => n.CitaId).HasColumnName("cita_id");
        builder.Property(n => n.PsicologoId).HasColumnName("psicologo_id");
        builder.Property(n => n.ResumenSesion).HasColumnName("resumen_sesion");
        builder.Property(n => n.EstadoAnimo).HasColumnName("estado_animo");
        builder.Property(n => n.NivelAnsiedad).HasColumnName("nivel_ansiedad");
        builder.Property(n => n.AvanceObjetivos).HasColumnName("avance_objetivos");
        builder.Property(n => n.TareasAsignadas).HasColumnName("tareas_asignadas");
        builder.Property(n => n.Observaciones).HasColumnName("observaciones");
        builder.Property(n => n.PlanProximaSesion).HasColumnName("plan_proxima_sesion");
        builder.Property(n => n.EvaluacionRiesgo).HasColumnName("evaluacion_riesgo");
        builder.Property(n => n.NivelRiesgo).HasColumnName("nivel_riesgo");
        builder.Property(n => n.AccionesRiesgo).HasColumnName("acciones_riesgo");
        builder.Property(n => n.Finalizada).HasColumnName("finalizada");
        builder.Property(n => n.CreadoEn).HasColumnName("creado_en");
        builder.Property(n => n.ActualizadoEn).HasColumnName("actualizado_en");

        builder.Property(n => n.TecnicasUsadas)
            .HasColumnName("tecnicas_usadas");

        builder.HasOne(n => n.Cita)
            .WithOne(c => c.Nota)
            .HasForeignKey<NotaSesion>(n => n.CitaId);

        builder.Ignore(n => n.DomainEvents);
    }
}