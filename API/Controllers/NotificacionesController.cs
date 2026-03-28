using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoCitas.Application.Common.Interfaces;

namespace PsychoCitas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class NotificacionesController(INotificationService notificationService) : ControllerBase
{
    [HttpPost("test/email")]
    public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request, CancellationToken ct)
    {
        await notificationService.EnviarEmailAsync(request.Email, request.Asunto, request.Contenido, ct);
        return Ok(new { ok = true, canal = "email" });
    }

    [HttpPost("test/sms")]
    public async Task<IActionResult> TestSms([FromBody] TestPhoneRequest request, CancellationToken ct)
    {
        await notificationService.EnviarSmsAsync(request.Telefono, request.Mensaje, ct);
        return Ok(new { ok = true, canal = "sms" });
    }

    [HttpPost("test/whatsapp")]
    public async Task<IActionResult> TestWhatsApp([FromBody] TestPhoneRequest request, CancellationToken ct)
    {
        await notificationService.EnviarWhatsAppAsync(request.Telefono, request.Mensaje, ct);
        return Ok(new { ok = true, canal = "whatsapp" });
    }
}

public record TestEmailRequest(string Email, string Asunto, string Contenido);
public record TestPhoneRequest(string Telefono, string Mensaje);