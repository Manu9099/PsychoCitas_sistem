using PsychoCitas.Domain.Enums;

namespace PsychoCitas.Domain.Entities;

public class EventoPago : BaseEntity
{
    public Guid IntentoPagoId { get; private set; }
    public ProveedorPago Proveedor { get; private set; }
    public string TipoEvento { get; private set; } = string.Empty;
    public string PayloadRaw { get; private set; } = string.Empty;
    public string? ProviderEventId { get; private set; }

    public IntentoPago? IntentoPago { get; private set; }

    protected EventoPago() { }

    public static EventoPago Crear(
        Guid intentoPagoId,
        ProveedorPago proveedor,
        string tipoEvento,
        string payloadRaw,
        string? providerEventId = null)
    {
        return new EventoPago
        {
            IntentoPagoId = intentoPagoId,
            Proveedor = proveedor,
            TipoEvento = tipoEvento,
            PayloadRaw = payloadRaw,
            ProviderEventId = providerEventId
        };
    }
}

