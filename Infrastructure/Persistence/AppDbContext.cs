using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistoriaClinica> HistoriasClinicas => Set<HistoriaClinica>();
    public DbSet<NotaSesion> NotasSesion => Set<NotaSesion>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<DocumentoPaciente> DocumentosPaciente => Set<DocumentoPaciente>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
    builder.HasPostgresEnum<EstadoCita>();
    builder.HasPostgresEnum<Modalidad>();
    builder.HasPostgresEnum<TipoSesion>();

    builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Entities.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.ActualizadoEn = DateTime.UtcNow;

        return base.SaveChangesAsync(ct);
    }
}