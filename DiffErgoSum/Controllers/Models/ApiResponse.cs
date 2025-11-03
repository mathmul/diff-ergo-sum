namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Base class for all API responses, providing a common error structure.
/// </summary>
public abstract record ApiResponse(
    [property: JsonPropertyName("error")] string? Error = null,
    [property: JsonPropertyName("message")] string? Message = null
)
{
    [JsonIgnore]
    public bool Success => string.IsNullOrEmpty(Error);
}
