using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICitaRepository Citas { get; }
    IPacienteRepository Pacientes { get; }
    INotaSesionRepository Notas { get; }
    IUsuarioRepository Usuarios { get; }
    IPagoRepository Pagos { get; }
    INotificacionRepository Notificaciones { get; }
    IDocumentoPacienteRepository DocumentosPaciente { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    IIntentoPagoRepository IntentosPago { get; }
    IEventoPagoRepository EventosPago { get; }
}