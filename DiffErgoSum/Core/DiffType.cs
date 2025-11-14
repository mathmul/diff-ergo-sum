namespace DiffErgoSum.Core;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the possible outcomes of a diff comparison between two Base64 inputs.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiffType
{
    Equals = 0,
    SizeDoNotMatch = 1,
    ContentDoNotMatch = 2,
}
