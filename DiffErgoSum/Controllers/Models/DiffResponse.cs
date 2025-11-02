namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

public class DiffResponse
{
    [JsonPropertyName("diffResultType")]
    public string DiffResultType { get; set; } = string.Empty;

    [JsonPropertyName("diffs")]
    public List<DiffSegmentDto>? Diffs { get; set; }
}

public class DiffSegmentDto
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }
}
