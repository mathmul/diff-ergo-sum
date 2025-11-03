namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a specific segment where two Base64 inputs differ.
/// </summary>
/// <remarks>
/// Each diff segment indicates a continuous range of bytes that differ
/// between the left and right payloads.
/// </remarks>
public record DiffResponseSegmentDto(
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("length")] int Length
);
