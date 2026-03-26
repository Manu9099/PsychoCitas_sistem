using System.Net;
using System.Text.Json;
using FluentValidation;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex, logger); }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
    {
        var (statusCode, title, errors) = ex switch
        {
            ValidationException vex => (HttpStatusCode.BadRequest, "Error de validación",
                vex.Errors.ToDictionary(e => e.PropertyName, e => e.Errors.ToArray())),
            NotFoundException => (HttpStatusCode.NotFound, ex.Message,
                (IDictionary<string, string[]>?)null),
            DomainException => (HttpStatusCode.UnprocessableEntity, ex.Message,
                (IDictionary<string, string[]>?)null),
            ConflictoAgendaException => (HttpStatusCode.Conflict, ex.Message,
                (IDictionary<string, string[]>?)null),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor",
                (IDictionary<string, string[]>?)null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(ex, "Unhandled exception");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { title, status = (int)statusCode, errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
