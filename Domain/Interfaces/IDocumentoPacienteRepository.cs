using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface IDocumentoPacienteRepository : IRepository<DocumentoPaciente>
{
    Task<List<DocumentoPaciente>> GetByPacienteIdAsync(Guid pacienteId, CancellationToken ct = default);
    Task<DocumentoPaciente?> GetActivoByIdAsync(Guid documentoId, CancellationToken ct = default);
}