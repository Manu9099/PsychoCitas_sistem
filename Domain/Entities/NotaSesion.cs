using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.Domain.Entities;

public class NotaSesion : BaseEntity
{
    public Guid CitaId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public string? ResumenSesion { get; private set; }
    public List<string> TecnicasUsadas { get; private set; } = [];
    public int? EstadoAnimo { get; private set; }     // 1-10
    public int? NivelAnsiedad { get; private set; }   // 0-10
    public string? AvanceObjetivos { get; private set; }
    public string? TareasAsignadas { get; private set; }
    public string? Observaciones { get; private set; }
    public string? PlanProximaSesion { get; private set; }
    public bool EvaluacionRiesgo { get; private set; }
    public NivelRiesgo? NivelRiesgo { get; private set; }
    public string? AccionesRiesgo { get; private set; }
    public bool Finalizada { get; private set; }

    public Cita? Cita { get; private set; }

    protected NotaSesion() { }

    public static NotaSesion Crear(Guid citaId, Guid psicologoId)
        => new() { CitaId = citaId, PsicologoId = psicologoId };

    public void Actualizar(
        string? resumen, List<string>? tecnicas,
        int? estadoAnimo, int? nivelAnsiedad,
        string? avance, string? tareas, string? observaciones, string? planProxima)
    {
        if (Finalizada) throw new DomainException("La nota ya está finalizada y no puede editarse.");
        if (estadoAnimo is < 1 or > 10) throw new DomainException("El estado de ánimo debe ser entre 1 y 10.");
        if (nivelAnsiedad is < 0 or > 10) throw new DomainException("El nivel de ansiedad debe ser entre 0 y 10.");

        ResumenSesion = resumen;
        TecnicasUsadas = tecnicas ?? [];
        EstadoAnimo = estadoAnimo;
        NivelAnsiedad = nivelAnsiedad;
        AvanceObjetivos = avance;
        TareasAsignadas = tareas;
        Observaciones = observaciones;
        PlanProximaSesion = planProxima;
    }

    public void RegistrarRiesgo(NivelRiesgo nivel, string acciones)
    {
        EvaluacionRiesgo = true;
        NivelRiesgo = nivel;
        AccionesRiesgo = acciones;
    }

    public void Finalizar()
    {
        if (string.IsNullOrWhiteSpace(ResumenSesion))
            throw new DomainException("Debe completar el resumen antes de finalizar la nota.");
        Finalizada = true;
    }
}
