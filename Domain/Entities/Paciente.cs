namespace PsychoCitas.Domain.Entities;

public class Paciente : BaseEntity
{
    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public DateOnly? FechaNacimiento { get; private set; }
    public string? Genero { get; private set; }
    public string? Dni { get; private set; }
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public string? TelefonoEmergencia { get; private set; }
    public string? ContactoEmergencia { get; private set; }
    public string? Ocupacion { get; private set; }
    public string? EstadoCivil { get; private set; }
    public string? Direccion { get; private set; }
    public string? ReferidoPor { get; private set; }
    public bool Activo { get; private set; } = true;

    // Navegación
    public HistoriaClinica? HistoriaClinica { get; private set; }
    public ICollection<Cita> Citas { get; private set; } = [];
    public ICollection<DocumentoPaciente> Documentos { get; private set; } = [];

    protected Paciente() { }

    public static Paciente Create(
        string nombres, string apellidos, string? email, string? telefono,
        string? dni = null, DateOnly? fechaNacimiento = null, string? genero = null,
        string? ocupacion = null, string? referidoPor = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombres);
        ArgumentException.ThrowIfNullOrWhiteSpace(apellidos);

        var paciente = new Paciente
        {
            Nombres = nombres.Trim(),
            Apellidos = apellidos.Trim(),
            Email = email?.Trim().ToLower(),
            Telefono = telefono?.Trim(),
            Dni = dni?.Trim(),
            FechaNacimiento = fechaNacimiento,
            Genero = genero,
            Ocupacion = ocupacion,
            ReferidoPor = referidoPor
        };

        paciente.AddDomainEvent(new PacienteRegistradoEvent(paciente.Id, paciente.NombreCompleto));
        return paciente;
    }

    public void Actualizar(string nombres, string apellidos, string? email,
        string? telefono, string? ocupacion, string? estadoCivil, string? direccion)
    {
        Nombres = nombres.Trim();
        Apellidos = apellidos.Trim();
        Email = email?.Trim().ToLower();
        Telefono = telefono?.Trim();
        Ocupacion = ocupacion;
        EstadoCivil = estadoCivil;
        Direccion = direccion;
    }

    public void Desactivar() => Activo = false;

    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public int? Edad => FechaNacimiento.HasValue
        ? (int)((DateTime.Today - FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25)
        : null;
}

// Domain Events
public record PacienteRegistradoEvent(Guid PacienteId, string NombreCompleto) : IDomainEvent;
