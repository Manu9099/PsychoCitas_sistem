using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x => x.Rol).HasColumnName("rol").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Activo).HasColumnName("activo").IsRequired();
        builder.Property(x => x.CreadoEn).HasColumnName("creado_en");
        builder.Property(x => x.ActualizadoEn).HasColumnName("actualizado_en");

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.NombreUsuario).IsUnique();

        builder.Ignore(x => x.DomainEvents);
    }
}