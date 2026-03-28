using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.HasIndex(n => new { n.CitaId, n.Canal, n.Tipo })
            .IsUnique();

        builder.Property(n => n.Canal)
            .HasMaxLength(20);

        builder.Property(n => n.Tipo)
            .HasMaxLength(40);

        builder.Property(n => n.Estado)
            .HasMaxLength(20);
    }
}