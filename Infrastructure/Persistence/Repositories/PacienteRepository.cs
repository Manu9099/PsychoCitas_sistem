using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class PacienteRepository(AppDbContext context)
    : BaseRepository<Paciente>(context), IPacienteRepository
{
    public async Task<Paciente?> GetByDniAsync(string dni, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.Dni == dni, ct);

    public async Task<Paciente?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(
            p => p.Email != null && p.Email == email.Trim().ToLower(),
            ct);

    public async Task<List<Paciente>> BuscarAsync(string termino, CancellationToken ct = default)
    {
        var t = termino.ToLower().Trim();

        return await _dbSet
            .Where(p => p.Activo &&
                (p.Nombres.ToLower().Contains(t) ||
                 p.Apellidos.ToLower().Contains(t) ||
                 (p.Dni != null && p.Dni.Contains(t)) ||
                 (p.Email != null && p.Email.Contains(t))))
            .OrderBy(p => p.Apellidos)
            .ThenBy(p => p.Nombres)
            .Take(20)
            .ToListAsync(ct);
    }

    public async Task<Paciente?> GetConHistoriaAsync(Guid id, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.HistoriaClinica)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
}