namespace PsychoCitas.Domain.Entities;

public class Notificacion : BaseEntity
{
    public Guid CitaId { get; private set; }
    public Guid PacienteId { get; private set; }
    public string Canal { get; private set; } = string.Empty;  // email|whatsapp|sms
    public string Tipo { get; private set; } = string.Empty;   // recordatorio_24h|confirmacion|cancelacion
    public string Estado { get; private set; } = "pendiente";
    public DateTime ProgramadaPara { get; private set; }
    public DateTime? EnviadaEn { get; private set; }
    public string? Mensaje { get; private set; }
    public string? ErrorDetalle { get; private set; }
    public int Intentos { get; private set; }

    public Cita? Cita { get; private set; }

    protected Notificacion() { }

    public static Notificacion Crear(Guid citaId, Guid pacienteId, string canal,
        string tipo, DateTime programadaPara, string mensaje)
        => new()
        {
            CitaId = citaId, PacienteId = pacienteId,
            Canal = canal, Tipo = tipo,
            ProgramadaPara = programadaPara, Mensaje = mensaje
        };

    public void MarcarEnviada() { Estado = "enviada"; EnviadaEn = DateTime.UtcNow; }
    public void MarcarFallida(string error) { Estado = "fallida"; ErrorDetalle = error; Intentos++; }
    public void Reintentar() { Estado = "pendiente"; Intentos++; }
}
