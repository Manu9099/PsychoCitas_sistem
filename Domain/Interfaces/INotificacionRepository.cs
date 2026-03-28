using PsychoCitas.Domain.Entities;

namespace PsychoCitas.Domain.Interfaces;

public interface INotificacionRepository : IRepository<Notificacion>
{
    Task<bool> ExisteDuplicadoAsync(Guid citaId, string canal, string tipo, CancellationToken ct = default);

    Task<List<Notificacion>> GetPendientesConDestinoAsync(DateTime hasta, CancellationToken ct = default);
}