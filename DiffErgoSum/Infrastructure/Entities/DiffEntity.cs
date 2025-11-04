namespace DiffErgoSum.Infrastructure.Entities;

using System.ComponentModel.DataAnnotations;

public class DiffEntity
{
    [Key]
    public int Id { get; set; }
    public string? Left { get; set; }
    public string? Right { get; set; }
}
