using Microsoft.EntityFrameworkCore.Storage;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;
using PsychoCitas.Infrastructure.Persistence.Repositories;

namespace PsychoCitas.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public ICitaRepository Citas { get; } = new CitaRepository(context);
    public IPacienteRepository Pacientes { get; } = new PacienteRepository(context);
    public INotaSesionRepository Notas { get; } = new NotaSesionRepository(context);
    public IUsuarioRepository Usuarios { get; } = new UsuarioRepository(context);
    public INotificacionRepository Notificaciones { get; } = new NotificacionRepository(context);
    public IDocumentoPacienteRepository DocumentosPaciente { get; } = new DocumentoPacienteRepository(context);
    public IPagoRepository Pagos { get; } = new PagoRepository(context);

     public IIntentoPagoRepository IntentosPago { get; } = new IntentoPagoRepository(context);
    public IEventoPagoRepository EventosPago { get; } = new EventoPagoRepository(context);

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        var entities = context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity be && be.DomainEvents.Any())
            .Select(e => (BaseEntity)e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        await context.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await context.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No hay transacción activa.");

        await context.SaveChangesAsync(ct);
        await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(ct);
    }

    public void Dispose() => _transaction?.Dispose();
}