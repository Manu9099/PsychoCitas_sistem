using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class HistoriaClinicaConfiguration : IEntityTypeConfiguration<HistoriaClinica>
{
    public void Configure(EntityTypeBuilder<HistoriaClinica> builder)
    {
        builder.ToTable("historia_clinica");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasColumnName("id");
        builder.Property(h => h.PacienteId).HasColumnName("paciente_id");
        builder.Property(h => h.PsicologoId).HasColumnName("psicologo_id");
        builder.Property(h => h.FechaIngreso).HasColumnName("fecha_ingreso");
        builder.Property(h => h.FechaAlta).HasColumnName("fecha_alta");
        builder.Property(h => h.MotivoConsulta).HasColumnName("motivo_consulta");
        builder.Property(h => h.DiagnosticoInicial).HasColumnName("diagnostico_inicial");
        builder.Property(h => h.DiagnosticoCie11).HasColumnName("diagnostico_cie11");
        builder.Property(h => h.ObjetivosTerapeuticos).HasColumnName("objetivos_terapeuticos");
        builder.Property(h => h.TratamientosPrevios).HasColumnName("tratamientos_previos");
        builder.Property(h => h.MedicacionActual).HasColumnName("medicacion_actual");
       builder.Property(h => h.AntecedentePersonales).HasColumnName("antecedentes_personales");
        builder.Property(h => h.AntecedentesFamiliares).HasColumnName("antecedentes_familiares");
        builder.Property(h => h.Alergias).HasColumnName("alergias");
        builder.Property(h => h.ObservacionesIniciales).HasColumnName("observaciones_iniciales");
        builder.Property(h => h.CreadoEn).HasColumnName("creado_en");
        builder.Property(h => h.ActualizadoEn).HasColumnName("actualizado_en");

        builder.HasOne(h => h.Paciente)
            .WithOne(p => p.HistoriaClinica)
            .HasForeignKey<HistoriaClinica>(h => h.PacienteId);

        builder.Ignore(h => h.DomainEvents);
    }
}