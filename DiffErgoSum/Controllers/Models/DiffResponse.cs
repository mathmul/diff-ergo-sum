namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

public class DiffResponse
{
    [JsonPropertyName("diffResultType")]
    public string DiffResultType { get; set; } = string.Empty;

    [JsonPropertyName("diffs")]
    public List<DiffResponseSegmentDto>? Diffs { get; set; }
}
