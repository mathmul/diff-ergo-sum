namespace DiffErgoSum.Controllers.Models;

using System.Text.Json.Serialization;

public class DiffResponseSegmentDto
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }
}
