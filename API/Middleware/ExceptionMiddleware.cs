using System.Net;
using System.Text.Json;
using FluentValidation;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
    {
        (HttpStatusCode statusCode, string title, IDictionary<string, string[]>? errors) = ex switch
        {
            ValidationException vex => (
                HttpStatusCode.BadRequest,
                "Error de validación",
                (IDictionary<string, string[]>)vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),

            NotFoundException => (
                HttpStatusCode.NotFound,
                ex.Message,
                null
            ),
            UnauthorizedAccessException => ( 
                HttpStatusCode.Unauthorized, ex.Message, 
                null ),

            DomainException => (
                HttpStatusCode.UnprocessableEntity,
                ex.Message,
                null
            ),

            ConflictoAgendaException => (
                HttpStatusCode.Conflict,
                ex.Message,
                null
            ),
            ConflictException => ( 
                HttpStatusCode.Conflict, 
                ex.Message, 
                null
                 ),
             
            _ => (
                HttpStatusCode.InternalServerError,
                "Error interno del servidor",
                null
            )
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(ex, "Unhandled exception");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            title,
            status = (int)statusCode,
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            )
        );
    }
}