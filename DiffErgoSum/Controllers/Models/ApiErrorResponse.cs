namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a standardized API error response.
/// </summary>
/// <remarks>
/// Every error response should include a machine-readable <c>error</c> code
/// and a human-readable <c>message</c>.
/// </remarks>
public record ApiErrorResponse(
    [property: JsonPropertyName("error")] string ErrorCode,
    [property: JsonPropertyName("message")] string ErrorMessage
) : ApiResponse
{
    /// <summary>
    /// Creates an <see cref="ApiErrorResponse"/> from an exception, using its type name as the error code.
    /// </summary>
    public static ApiErrorResponse FromException(Exception ex) =>
        new(ex.GetType().Name, ex.Message);
}
