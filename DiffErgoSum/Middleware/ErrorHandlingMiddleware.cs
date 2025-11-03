namespace DiffErgoSum.Middleware;

using System.Net;

using DiffErgoSum.Controllers.Exceptions;
using DiffErgoSum.Controllers.Models;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (HttpException hex)
        {
            _logger.LogError(hex, "Http exception during {Path}: {Error}", context.Request.Path, hex.ErrorCode);

            context.Response.StatusCode = hex.StatusCode;
            context.Response.ContentType = "application/problem+json; charset=utf-8";
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started. Cannot write error for {Path}", context.Request.Path);
                return;
            }
            await context.Response.WriteAsJsonAsync(hex.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception during {Path}", context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json; charset=utf-8";
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started. Cannot write error for {Path}", context.Request.Path);
                return;
            }
            await context.Response.WriteAsJsonAsync(ApiErrorResponse.FromException(ex));
        }
    }
}
