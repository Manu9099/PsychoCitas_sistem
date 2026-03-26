namespace PsychoCitas.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICitaRepository Citas { get; }
    IPacienteRepository Pacientes { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
