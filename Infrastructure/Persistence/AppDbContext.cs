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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<EstadoCita>("estado_cita");
        modelBuilder.HasPostgresEnum<Modalidad>("modalidad");
        modelBuilder.HasPostgresEnum<TipoSesion>("tipo_sesion");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Entities.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.ActualizadoEn = DateTime.UtcNow;

        return base.SaveChangesAsync(ct);
    }
}