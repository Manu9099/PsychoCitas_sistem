using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface IPacienteRepository : IRepository<Paciente>
{
    Task<Paciente?> GetByDniAsync(string dni, CancellationToken ct = default);
    Task<Paciente?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<List<Paciente>> BuscarAsync(string termino, CancellationToken ct = default);
    Task<Paciente?> GetConHistoriaAsync(Guid id, CancellationToken ct = default);
}