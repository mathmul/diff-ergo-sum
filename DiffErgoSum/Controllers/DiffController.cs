using DiffErgoSum.Controllers.Models;
using DiffErgoSum.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace DiffErgoSum.Controllers;

[ApiController]
[Route("api/v1/diff/{id}")]
public class DiffController : ControllerBase
{
    private static readonly InMemoryDiffRepository _repo = new();

    public static void ResetRepository() => _repo.Clear();

    [HttpPut("left")]
    public IActionResult UploadLeft(int id, [FromBody] DiffRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Data))
            return BadRequest();

        _repo.SaveLeft(id, request.Data);
        return Created(string.Empty, null);
    }

    [HttpPut("right")]
    public IActionResult UploadRight(int id, [FromBody] DiffRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Data))
            return BadRequest();

        _repo.SaveRight(id, request.Data);
        return Created(string.Empty, null);
    }

    [HttpGet]
    public IActionResult GetDiff(int id)
    {
        var pair = _repo.Get(id);
        if (pair == null || string.IsNullOrEmpty(pair.Value.Left) || string.IsNullOrEmpty(pair.Value.Right))
            return NotFound();

        // Implementation for diffing will come later
        return Ok(new { message = "Not implemented yet" });
    }
}
