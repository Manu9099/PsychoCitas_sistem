using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class EventoPagoRepository(AppDbContext context)
    : BaseRepository<EventoPago>(context), IEventoPagoRepository
{
    public async Task<bool> ExisteEventoProveedorAsync(string providerEventId, CancellationToken ct = default)
        => await _dbSet.AnyAsync(x => x.ProviderEventId == providerEventId, ct);
}