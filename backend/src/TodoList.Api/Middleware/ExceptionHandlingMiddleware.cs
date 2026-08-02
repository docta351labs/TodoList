using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Exceptions;
using TodoList.Domain.Exceptions;

namespace TodoList.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        string traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        ProblemDetails problemDetails = exception switch
        {
            ValidationException valEx => new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "One or more validation failures occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["errors"] = valEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray())
                }
            },
            NotFoundException notFoundEx => new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = notFoundEx.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },
            DomainException domainEx => new ProblemDetails
            {
                Title = "Domain Rule Violation",
                Detail = domainEx.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },
            _ => new ProblemDetails
            {
                Title = "An Error Occurred",
                Detail = "An unexpected internal server error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
