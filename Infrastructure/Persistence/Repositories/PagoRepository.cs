using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class PagoRepository(AppDbContext context)
    : BaseRepository<Pago>(context), IPagoRepository
{
    public async Task<Pago?> GetByCitaIdAsync(Guid citaId, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Cita)
            .FirstOrDefaultAsync(p => p.CitaId == citaId, ct);

    public async Task<Pago?> GetByIdConCitaAsync(Guid pagoId, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Cita)
            .FirstOrDefaultAsync(p => p.Id == pagoId, ct);

    public async Task<List<Pago>> GetByPacienteIdAsync(Guid pacienteId, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Cita)
            .Where(p => p.PacienteId == pacienteId)
            .OrderByDescending(p => p.PagadoEn ?? p.CreadoEn)
            .ToListAsync(ct);
}