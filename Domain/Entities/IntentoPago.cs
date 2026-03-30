using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Domain.Entities;

public class IntentoPago : BaseEntity
{
    public Guid PagoId { get; private set; }
    public ProveedorPago Proveedor { get; private set; }
    public EstadoIntentoPago Estado { get; private set; }
    public decimal Monto { get; private set; }
    public string Moneda { get; private set; } = "PEN";
    public string? ExternalReference { get; private set; }
    public string? CheckoutUrl { get; private set; }
    public string? ProviderPaymentId { get; private set; }
    public string? RawResponse { get; private set; }
    public DateTime? ExpiraEn { get; private set; }

    public Pago? Pago { get; private set; }

    protected IntentoPago() { }

    public static IntentoPago Crear(
        Guid pagoId,
        ProveedorPago proveedor,
        decimal monto,
        string moneda = "PEN",
        string? externalReference = null,
        string? checkoutUrl = null,
        DateTime? expiraEn = null)
    {
        return new IntentoPago
        {
            PagoId = pagoId,
            Proveedor = proveedor,
            Estado = EstadoIntentoPago.Creado,
            Monto = monto,
            Moneda = moneda,
            ExternalReference = externalReference,
            CheckoutUrl = checkoutUrl,
            ExpiraEn = expiraEn
        };
    }

    public void MarcarPendiente(string? rawResponse = null)
    {
        Estado = EstadoIntentoPago.Pendiente;
        RawResponse = rawResponse;
    }

    public void MarcarExitoso(string? providerPaymentId, string? rawResponse = null)
    {
        Estado = EstadoIntentoPago.Exitoso;
        ProviderPaymentId = providerPaymentId;
        RawResponse = rawResponse;
    }

    public void MarcarFallido(string? rawResponse = null)
    {
        Estado = EstadoIntentoPago.Fallido;
        RawResponse = rawResponse;
    }

    public void MarcarExpirado()
    {
        Estado = EstadoIntentoPago.Expirado;
    }

    public void MarcarCancelado()
    {
        Estado = EstadoIntentoPago.Cancelado;
    }
}
