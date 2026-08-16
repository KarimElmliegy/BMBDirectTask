using BMBAssessment.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BMBAssessment.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }
    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            RequestValidationException validation => (400, "Validation failed", validation.Message),
            UnauthorizedException unauthorized => (401, "Unauthorized", unauthorized.Message),
            CustomerBannedException banned => (403, "Customer banned", banned.Message),
            NotFoundException notFound => (404, "Not found", notFound.Message),
            ConflictException conflict => (409, "Conflict", conflict.Message),
            DbUpdateConcurrencyException => (409, "Conflict", "Inventory or order data changed. Refresh and try again."),
            DbUpdateException => (409, "Conflict", "The request conflicts with existing data."),
            _ => (500, "Server error", "An unexpected error occurred.")
        };
        if (status == 500) 
            _logger.LogError(exception, "Unhandled request exception");
        
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        
        await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title, Detail = detail, Instance = context.Request.Path });
    }
}
