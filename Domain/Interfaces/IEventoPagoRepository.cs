using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface IEventoPagoRepository : IRepository<EventoPago>
{
    Task<bool> ExisteEventoProveedorAsync(string providerEventId, CancellationToken ct = default);
}