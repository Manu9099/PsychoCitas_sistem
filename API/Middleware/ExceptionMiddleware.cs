using FluentValidation;
using Microsoft.AspNetCore.Http;
using PsychoCitas.API.Common;
using PsychoCitas.Domain.Exceptions;
using System.Net;
using System.Text.Json;

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
            logger.LogError(ex, "Unhandled exception");

            var response = MapException(ex, context);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.Status;

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private static ApiErrorResponse MapException(Exception ex, HttpContext context)
    {
        var traceId = context.TraceIdentifier;

        return ex switch
        {
            ValidationException ve => new ApiErrorResponse
            {
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Errors = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()),
                TraceId = traceId
            },

            NotFoundException nfe => new ApiErrorResponse
            {
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = nfe.Message,
                TraceId = traceId
            },

            ConflictoAgendaException cae => new ApiErrorResponse
            {
                Title = "Schedule conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = cae.Message,
                TraceId = traceId
            },

            DomainException de => new ApiErrorResponse
            {
                Title = "Domain error",
                Status = StatusCodes.Status400BadRequest,
                Detail = de.Message,
                TraceId = traceId
            },

            UnauthorizedAccessException => new ApiErrorResponse
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "You are not authorized to perform this action.",
                TraceId = traceId
            },

            _ => new ApiErrorResponse
            {
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                TraceId = traceId
            }
        };
    }
}