using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class DocumentoPacienteRepository(AppDbContext context)
    : BaseRepository<DocumentoPaciente>(context), IDocumentoPacienteRepository
{
    public async Task<List<DocumentoPaciente>> GetByPacienteIdAsync(Guid pacienteId, CancellationToken ct = default)
        => await _dbSet
            .Where(d => d.PacienteId == pacienteId && d.Activo)
            .OrderByDescending(d => d.CreadoEn)
            .ToListAsync(ct);

    public async Task<DocumentoPaciente?> GetActivoByIdAsync(Guid documentoId, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(d => d.Id == documentoId && d.Activo, ct);
}