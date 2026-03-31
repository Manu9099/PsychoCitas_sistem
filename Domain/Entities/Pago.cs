using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.Domain.Entities;

public class Pago : BaseEntity
{
    public Guid CitaId { get; private set; }
    public Guid PacienteId { get; private set; }
    public decimal Monto { get; private set; }
    public decimal MontoPagado { get; private set; }
    public EstadoPago Estado { get; private set; }
    public string? MetodoPago { get; private set; }
    public string? NumeroOperacion { get; private set; }
    public string? Notas { get; private set; }
    public Guid? RegistradoPor { get; private set; }
   // public DateTime CreadoEn { get; private set; }
    public DateTime? PagadoEn { get; private set; }

    public Cita? Cita { get; private set; }

    protected Pago() { }

    public static Pago Crear(Guid citaId, Guid pacienteId, decimal monto, Guid? registradoPor = null)
    {
        if (monto <= 0) throw new DomainException("El monto debe ser mayor a cero.");

        return new Pago
        {
            CitaId = citaId,
            PacienteId = pacienteId,
            Monto = monto,
            Estado = EstadoPago.Pendiente,
            RegistradoPor = registradoPor,
            CreadoEn = DateTime.UtcNow
        };
    }

    public void Registrar(decimal montoPagado, string metodoPago, string? numeroOperacion = null, string? notas = null)
    {
        if (montoPagado <= 0) throw new DomainException("El monto pagado debe ser mayor a cero.");
        if (montoPagado > Monto) throw new DomainException("El monto pagado no puede superar el monto de la cita.");

        MontoPagado = montoPagado;
        MetodoPago = metodoPago;
        NumeroOperacion = numeroOperacion;
        Notas = notas;
        PagadoEn = DateTime.UtcNow;
        Estado = montoPagado >= Monto ? EstadoPago.Pagado : EstadoPago.Parcial;
    }

    public void Exonerar(string motivo)
    {
        Estado = EstadoPago.Exonerado;
        Notas = motivo;
    }

    public decimal Saldo => Monto - MontoPagado;
}