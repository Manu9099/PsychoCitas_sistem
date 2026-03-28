using Microsoft.EntityFrameworkCore;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Infrastructure.Persistence.Repositories;

public class CitaRepository(AppDbContext context) : BaseRepository<Cita>(context), ICitaRepository
{
    public async Task<List<Cita>> GetAgendaDiaAsync(Guid psicologoId, DateOnly fecha, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Paciente)
            .Include(c => c.Nota)
            .Include(c => c.Pago)
            .Where(c => c.PsicologoId == psicologoId && DateOnly.FromDateTime(c.FechaInicio) == fecha)
            .OrderBy(c => c.FechaInicio)
            .ToListAsync(ct);

    public async Task<List<Cita>> GetByPacienteAsync(Guid pacienteId, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Pago)
            .Where(c => c.PacienteId == pacienteId)
            .OrderByDescending(c => c.FechaInicio)
            .ToListAsync(ct);

    public async Task<Cita?> GetDetalleByIdAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Paciente)
            .Include(c => c.Nota)
            .Include(c => c.Pago)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExisteSolapamientoAsync(
        Guid psicologoId,
        DateTime inicio,
        DateTime fin,
        Guid? excluirId = null,
        CancellationToken ct = default) =>
        await _dbSet.AnyAsync(c =>
            c.PsicologoId == psicologoId &&
            (excluirId == null || c.Id != excluirId) &&
            c.Estado != EstadoCita.Cancelada &&
            c.Estado != EstadoCita.NoAsistio &&
            c.FechaInicio < fin &&
            c.FechaFin > inicio, ct);

    public async Task<int> ContarSesionesCompletadasAsync(Guid pacienteId, Guid psicologoId, CancellationToken ct = default) =>
        await _dbSet.CountAsync(c =>
            c.PacienteId == pacienteId &&
            c.PsicologoId == psicologoId &&
            c.Estado == EstadoCita.Completada, ct);

    public async Task<List<Cita>> GetProximasConRecordatorioAsync(
        DateTime desde,
        DateTime hasta,
        CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Paciente)
            .Where(c =>
                c.FechaInicio >= desde &&
                c.FechaInicio <= hasta &&
                (c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada))
            .ToListAsync(ct);
}