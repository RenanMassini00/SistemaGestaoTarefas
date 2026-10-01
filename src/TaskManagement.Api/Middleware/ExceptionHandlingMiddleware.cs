using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.Api.Middleware;
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (NotFoundException ex) { logger.LogWarning(ex, "Resource not found"); await Write(context, 404, "Resource not found", ex.Message); }
        catch (ArgumentException ex) { logger.LogWarning(ex, "Invalid request"); await Write(context, 400, "Invalid request", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Unhandled error"); await Write(context, 500, "Internal server error", "An unexpected error occurred."); }
    }
    private static Task Write(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status; return context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title, Detail = detail, Instance = context.Request.Path, Extensions = { ["traceId"] = context.TraceIdentifier } });
    }
}
