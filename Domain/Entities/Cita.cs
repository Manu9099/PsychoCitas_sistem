using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.Domain.Entities;

public class Cita : BaseEntity
{
    public Guid PacienteId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public TipoSesion TipoSesion { get; private set; }
    public Modalidad Modalidad { get; private set; }
    public EstadoCita Estado { get; private set; }
    public string? LinkVideollamada { get; private set; }
    public int? NumeroSesion { get; private set; }
    public bool EsPrimeraVez { get; private set; }
    public string? NotasPrevias { get; private set; }
    public string? MotivoCancelacion { get; private set; }
    public Guid? CanceladoPor { get; private set; }
    public DateTime? CanceladoEn { get; private set; }
    public Guid? RecurrenciaId { get; private set; }
    public Guid? CreadoPor { get; private set; }
   

    // Navegación
    public Paciente? Paciente { get; private set; }
    public NotaSesion? Nota { get; private set; }
    public Pago? Pago { get; private set; }
    public ICollection<Notificacion> Notificaciones { get; private set; } = [];

    protected Cita() { }

    public static Cita Crear(
        Guid pacienteId, Guid psicologoId,
        DateTime fechaInicio, DateTime fechaFin,
        TipoSesion tipoSesion, Modalidad modalidad,
        bool esPrimeraVez = false,
        string? linkVideollamada = null,
        string? notasPrevias = null,
        Guid? creadoPor = null,
        Guid? recurrenciaId = null)
    {
        if (fechaInicio >= fechaFin)
            throw new DomainException("La fecha de inicio debe ser anterior a la fecha fin.");

        if (fechaInicio <= DateTime.UtcNow)
            throw new DomainException("No se puede agendar una cita en el pasado.");

        if (modalidad == Modalidad.Videollamada && string.IsNullOrWhiteSpace(linkVideollamada))
            throw new DomainException("Las sesiones por videollamada requieren un link.");

        var cita = new Cita
        {
            PacienteId = pacienteId,
            PsicologoId = psicologoId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            TipoSesion = tipoSesion,
            Modalidad = modalidad,
            Estado = EstadoCita.Programada,
            EsPrimeraVez = esPrimeraVez,
            LinkVideollamada = linkVideollamada,
            NotasPrevias = notasPrevias,
            CreadoPor = creadoPor,
            RecurrenciaId = recurrenciaId
        };

        cita.AddDomainEvent(new CitaAgendadaEvent(cita.Id, pacienteId, psicologoId, fechaInicio));
        return cita;
    }

    public void Confirmar()
    {
        if (Estado != EstadoCita.Programada)
            throw new DomainException($"Solo se pueden confirmar citas en estado Programada. Estado actual: {Estado}");
        Estado = EstadoCita.Confirmada;
    }

    public void Completar(int? numeroSesion = null)
    {
        if (Estado is not (EstadoCita.Programada or EstadoCita.Confirmada))
            throw new DomainException("Solo se pueden completar citas programadas o confirmadas.");
        Estado = EstadoCita.Completada;
        NumeroSesion = numeroSesion;
        AddDomainEvent(new CitaCompletadaEvent(Id, PacienteId, PsicologoId));
    }

    public void Cancelar(string motivo, Guid canceladoPor)
    {
        if (Estado is EstadoCita.Completada or EstadoCita.Cancelada)
            throw new DomainException("No se puede cancelar una cita ya completada o cancelada.");
        Estado = EstadoCita.Cancelada;
        MotivoCancelacion = motivo;
        CanceladoPor = canceladoPor;
        CanceladoEn = DateTime.UtcNow;
        AddDomainEvent(new CitaCanceladaEvent(Id, PacienteId, motivo));
    }

    public void MarcarNoAsistio()
    {
        if (Estado is not (EstadoCita.Programada or EstadoCita.Confirmada))
            throw new DomainException("Estado inválido para marcar inasistencia.");
        Estado = EstadoCita.NoAsistio;
        AddDomainEvent(new CitaNoAsistioEvent(Id, PacienteId));
    }

    public TimeSpan Duracion => FechaFin - FechaInicio;
}

// Domain Events
public record CitaAgendadaEvent(Guid CitaId, Guid PacienteId, Guid PsicologoId, DateTime FechaInicio) : IDomainEvent;
public record CitaCompletadaEvent(Guid CitaId, Guid PacienteId, Guid PsicologoId) : IDomainEvent;
public record CitaCanceladaEvent(Guid CitaId, Guid PacienteId, string Motivo) : IDomainEvent;
public record CitaNoAsistioEvent(Guid CitaId, Guid PacienteId) : IDomainEvent;
