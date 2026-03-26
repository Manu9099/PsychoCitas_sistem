using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Domain.Entities;

public class HistoriaClinica : BaseEntity
{
    public Guid PacienteId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public string? MotivoConsulta { get; private set; }
    public string? DiagnosticoInicial { get; private set; }
    public string? DiagnosticoCie11 { get; private set; }
    public string? AntecedentePersonales { get; private set; }
    public string? AntecedentesFamiliares { get; private set; }
    public string? MedicacionActual { get; private set; }
    public string? Alergias { get; private set; }
    public string? TratamientosPrevios { get; private set; }
    public string? ObjetivosTerapeuticos { get; private set; }
    public string? ObservacionesIniciales { get; private set; }
    public DateOnly FechaIngreso { get; private set; }
    public DateOnly? FechaAlta { get; private set; }

    public Paciente? Paciente { get; private set; }

    protected HistoriaClinica() { }

    public static HistoriaClinica Crear(Guid pacienteId, Guid psicologoId, string? motivoConsulta,
        string? diagnosticoInicial = null, string? objetivosTerapeuticos = null)
    {
        return new HistoriaClinica
        {
            PacienteId = pacienteId,
            PsicologoId = psicologoId,
            MotivoConsulta = motivoConsulta,
            DiagnosticoInicial = diagnosticoInicial,
            ObjetivosTerapeuticos = objetivosTerapeuticos,
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today)
        };
    }

    public void ActualizarDiagnostico(string? diagnostico, string? cie11, string? objetivos)
    {
        DiagnosticoInicial = diagnostico;
        DiagnosticoCie11 = cie11;
        ObjetivosTerapeuticos = objetivos;
    }

    public void DarAlta() => FechaAlta = DateOnly.FromDateTime(DateTime.Today);
    public bool EstaActivo => !FechaAlta.HasValue;
}
