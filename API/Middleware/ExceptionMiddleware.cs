using System.Net;
using System.Text.Json;
using FluentValidation;
using PsychoCitas.Domain.Exceptions;

namespace PsychoCitas.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Validation Error",
                message = "Uno o más campos no son válidos.",
                status = 400,
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Not Found",
                message = ex.Message,
                status = 404
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized");

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Unauthorized",
                message = ex.Message,
                status = 401
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Internal Server Error",
                message = "Ocurrió un error inesperado.",
                status = 500
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}