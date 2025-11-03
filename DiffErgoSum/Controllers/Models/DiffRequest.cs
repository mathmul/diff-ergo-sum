namespace DiffErgoSum.Controllers.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the request body for uploading one side of a diff (left or right).
/// </summary>
/// <remarks>
/// The <c>data</c> field must be a valid Base64-encoded string.
/// Used by PUT endpoints: <c>/api/v1/diff/{id}/left</c> and <c>/api/v1/diff/{id}/right</c>.
/// </remarks>
public record DiffRequest(
    [property: Required]
    [property: RegularExpression(@"^[A-Za-z0-9+/=]+$", ErrorMessage = "Data must be Base64.")]
    [property: JsonPropertyName("data")]
    string Data
)
{
    // ASP.NET Core model binding requires this when deserializing JSON to a record
    public DiffRequest()
        : this(string.Empty) { }
}
