namespace DiffErgoSum.Api.Models;

using System.Text.Json.Serialization;

/// <summary>
/// RFC 9457 "Problem Details for HTTP APIs" error response.
/// </summary>
/// <remarks>
/// Visit https://datatracker.ietf.org/doc/html/rfc9457#section-3 for more details.
/// </remarks>
public record ProblemDetailsResponse(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("detail")] string? Detail = null,
    [property: JsonPropertyName("instance")] string? Instance = null
)
{
    // Optional extensions (RFC 9457 §3.2)
    // Optional extensions (RFC 9457 §3.2)
    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; init; }
}
