namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the API response returned by <c>GET /api/v1/diff/{id}</c>.
/// </summary>
/// <remarks>
/// Provides the comparison result type and an optional list of differing segments.
/// </remarks>
public record DiffResponse(
    [property: JsonPropertyName("diffResultType")] string DiffResultType,
    [property: JsonPropertyName("diffs")] List<DiffResponseSegmentDto>? Diffs
);
