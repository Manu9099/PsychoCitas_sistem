using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class NotificacionRepository(AppDbContext context)
    : BaseRepository<Notificacion>(context), INotificacionRepository
{
    public async Task<bool> ExisteDuplicadoAsync(
        Guid citaId,
        string canal,
        string tipo,
        CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(
            n => n.CitaId == citaId && n.Canal == canal && n.Tipo == tipo,
            ct);
    }

    public async Task<List<Notificacion>> GetPendientesConDestinoAsync(
        DateTime hasta,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(n => n.Cita!)
                .ThenInclude(c => c.Paciente)
            .Where(n =>
                (n.Estado == "pendiente" || n.Estado == "fallida") &&
                n.ProgramadaPara <= hasta &&
                n.Intentos < 3 &&
                n.Cita != null &&
                (n.Cita.Estado == EstadoCita.Programada || n.Cita.Estado == EstadoCita.Confirmada))
            .OrderBy(n => n.ProgramadaPara)
            .ToListAsync(ct);
    }
}