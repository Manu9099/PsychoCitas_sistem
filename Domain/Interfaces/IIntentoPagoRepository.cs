using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface IIntentoPagoRepository : IRepository<IntentoPago>
{
    Task<IntentoPago?> GetByIdConPagoAsync(Guid id, CancellationToken ct = default);
    Task<IntentoPago?> GetByExternalReferenceAsync(string externalReference, CancellationToken ct = default);
    Task<List<IntentoPago>> GetByPagoIdAsync(Guid pagoId, CancellationToken ct = default);
}
