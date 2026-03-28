using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class NotaSesionRepository(AppDbContext context) : BaseRepository<NotaSesion>(context), INotaSesionRepository
{
    public async Task<NotaSesion?> GetByCitaIdAsync(Guid citaId, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(n => n.CitaId == citaId, ct);
}