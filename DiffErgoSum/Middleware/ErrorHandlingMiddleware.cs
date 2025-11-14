namespace DiffErgoSum.Middleware;

using System.Net;

using DiffErgoSum.Api.Exceptions;
using DiffErgoSum.Api.Models;

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
            _logger.LogError(hex, "Http exception during {Path}: {Error}", context.Request.Path, hex.GetType());

            context.Response.StatusCode = hex.StatusCode;
            context.Response.ContentType = "application/problem+json; charset=utf-8";
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started. Cannot write error for {Path}", context.Request.Path);
                return;
            }
            await context.Response.WriteAsJsonAsync(hex.ToProblemDetails(context.Request.Path));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception during {Path}", context.Request.Path);

            var fallback = new ProblemDetailsResponse(
                Type: "about:blank",
                Title: "Internal Server Error",
                Status: StatusCodes.Status500InternalServerError,
                Detail: ex.Message,
                Instance: context.Request.Path
            );

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json; charset=utf-8";
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started. Cannot write error for {Path}", context.Request.Path);
                return;
            }
            await context.Response.WriteAsJsonAsync(fallback);
        }
    }
}
