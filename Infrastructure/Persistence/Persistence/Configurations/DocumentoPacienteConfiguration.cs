using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class DocumentoPacienteConfiguration : IEntityTypeConfiguration<DocumentoPaciente>
{
    public void Configure(EntityTypeBuilder<DocumentoPaciente> builder)
    {
        builder.ToTable("documentos_paciente");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NombreOriginal)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.NombreArchivo)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Extension)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.UrlStorage)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Observaciones)
            .HasMaxLength(500);

        builder.Property(x => x.TamanoBytes)
            .IsRequired();

        builder.HasIndex(x => x.PacienteId);

        builder.HasOne(x => x.Paciente)
            .WithMany(p => p.Documentos)
            .HasForeignKey(x => x.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}