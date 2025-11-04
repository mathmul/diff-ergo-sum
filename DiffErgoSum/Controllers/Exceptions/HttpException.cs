namespace DiffErgoSum.Controllers.Exceptions;

using System;

using DiffErgoSum.Controllers.Models;

/// <summary>
/// Base class for HTTP exceptions with specific status codes and error details.
/// </summary>
public class HttpException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }
    public string Type { get; }
    public string? Detail { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP Status Code.</param>
    /// <param name="title">0...</param>
    /// <param name="type">1...</param>
    /// <param name="detail">2...</param>
    public HttpException(
        int statusCode = 500,
        string title = "Internal Server Error",
        string type = "about:blank",
        string? detail = null
    )
        : base(detail ?? title)
    {
        StatusCode = statusCode;
        Title = title;
        Type = type;
        Detail = detail;
    }

    /// <summary>Converts this exception to a standardized API error response.</summary>
    /// <returns>An <see cref="ProblemDetailsResponse"/> containing the error code and message.</returns>
    public ProblemDetailsResponse ToProblemDetails(string? instance = null) =>
        new(Type, Title, StatusCode, Detail ?? Message, instance);
}
