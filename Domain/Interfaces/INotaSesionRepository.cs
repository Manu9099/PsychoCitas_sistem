using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface INotaSesionRepository : IRepository<NotaSesion>
{
    Task<NotaSesion?> GetByCitaIdAsync(Guid citaId, CancellationToken ct = default);
}