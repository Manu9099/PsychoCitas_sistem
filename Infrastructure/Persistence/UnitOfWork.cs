using Microsoft.EntityFrameworkCore.Storage;
using PsychoCitas.Domain.Interfaces;
using PsychoCitas.Infrastructure.Persistence.Repositories;

namespace PsychoCitas.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public ICitaRepository Citas { get; } = new CitaRepository(context);
    public IPacienteRepository Pacientes { get; } = new PacienteRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Dispatch domain events before saving
        var entities = context.ChangeTracker.Entries<Domain.Entities.BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        var result = await context.SaveChangesAsync(ct);
        // TODO: Dispatch events via MediatR/EventBus
        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await context.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null) throw new InvalidOperationException("No hay transacción activa.");
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
