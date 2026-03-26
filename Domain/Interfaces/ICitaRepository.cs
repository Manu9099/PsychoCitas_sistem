using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Domain.Interfaces;

public interface ICitaRepository : IRepository<Cita>
{
    Task<IEnumerable<Cita>> GetAgendaDiaAsync(Guid psicologoId, DateOnly fecha, CancellationToken ct = default);
    Task<IEnumerable<Cita>> GetByPacienteAsync(Guid pacienteId, CancellationToken ct = default);
    Task<bool> ExisteSolapamientoAsync(Guid psicologoId, DateTime inicio, DateTime fin, Guid? excluirId = null, CancellationToken ct = default);
    Task<int> ContarSesionesCompletadasAsync(Guid pacienteId, Guid psicologoId, CancellationToken ct = default);
    Task<IEnumerable<Cita>> GetProximasConRecordatorioAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);
}
