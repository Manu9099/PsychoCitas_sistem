using Microsoft.AspNetCore.Http;
using PsychoCitas.Domain.Enums;

namespace PsychoCitas.API.Models;

public class SubirDocumentoPacienteRequest
{
    public IFormFile Archivo { get; set; } = default!;
    public TipoDocumentoPaciente Tipo { get; set; }
    public string? Observaciones { get; set; }
}