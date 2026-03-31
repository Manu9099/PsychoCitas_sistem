using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Infrastructure.Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("pagos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.CitaId).HasColumnName("cita_id");
        builder.Property(p => p.PacienteId).HasColumnName("paciente_id");
        builder.Property(p => p.Monto).HasColumnName("monto");
        builder.Property(p => p.MontoPagado).HasColumnName("monto_pagado");
        builder.Property(p => p.MetodoPago).HasColumnName("metodo_pago");
        builder.Property(p => p.Estado).HasColumnName("estado");
        builder.Property(p => p.NumeroOperacion).HasColumnName("numero_operacion");
        builder.Property(p => p.Notas).HasColumnName("notas");
        builder.Property(p => p.PagadoEn).HasColumnName("pagado_en");
        builder.Property(p => p.RegistradoPor).HasColumnName("registrado_por");

        builder.Property(p => p.CreadoEn).HasColumnName("creado_en");
        builder.Property(p => p.ActualizadoEn).HasColumnName("actualizado_en");

        builder.HasOne(p => p.Cita)
            .WithOne(c => c.Pago) // o WithMany(c => c.Pagos), según tu modelo final
            .HasForeignKey<Pago>(p => p.CitaId);

        builder.Ignore(p => p.DomainEvents);
    }
}