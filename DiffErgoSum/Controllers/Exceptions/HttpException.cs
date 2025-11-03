namespace DiffErgoSum.Controllers.Exceptions;

using System;

using DiffErgoSum.Controllers.Models;

/// <summary>
/// Base class for HTTP exceptions with specific status codes and error details.
/// </summary>
public class HttpException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP Status Code.</param>
    /// <param name="errorCode">Machine-readable Error Code.</param>
    /// <param name="errorMessage">Human-readable Error Message.</param>
    public HttpException(
        int statusCode = 500,
        string errorCode = "ApiError",
        string errorMessage = "Unknown API error"
    )
        : base(errorMessage) // Maps to .Message
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>Converts this exception to a standardized API error response.</summary>
    /// <returns>An <see cref="ApiErrorResponse"/> containing the error code and message.</returns>
    public ApiErrorResponse ToResponse() =>
        new(ErrorCode, Message);
}
