using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class IntentoPagoRepository(AppDbContext context)
    : BaseRepository<IntentoPago>(context), IIntentoPagoRepository
{
    public async Task<IntentoPago?> GetByIdConPagoAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.Include(x => x.Pago).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IntentoPago?> GetByExternalReferenceAsync(string externalReference, CancellationToken ct = default)
        => await _dbSet.Include(x => x.Pago)
            .FirstOrDefaultAsync(x => x.ExternalReference == externalReference, ct);

    public async Task<List<IntentoPago>> GetByPagoIdAsync(Guid pagoId, CancellationToken ct = default)
        => await _dbSet.Where(x => x.PagoId == pagoId)
            .OrderByDescending(x => x.CreadoEn)
            .ToListAsync(ct);
}