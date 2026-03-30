using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface IPagoRepository : IRepository<Pago>
{
    Task<Pago?> GetByCitaIdAsync(Guid citaId, CancellationToken ct = default);
    Task<Pago?> GetByIdConCitaAsync(Guid pagoId, CancellationToken ct = default);
    Task<List<Pago>> GetByPacienteIdAsync(Guid pacienteId, CancellationToken ct = default);
}