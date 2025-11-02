namespace DiffErgoSum.Controllers.Models;

using System.ComponentModel.DataAnnotations;

public class DiffRequest
{
    [Required]
    public string? Data { get; set; }
}
