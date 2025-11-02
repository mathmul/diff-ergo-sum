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

        try
        {
            var leftBytes = Convert.FromBase64String(pair.Value.Left);
            var rightBytes = Convert.FromBase64String(pair.Value.Right);

            var service = new Application.DiffService();
            var result = service.Compare(leftBytes, rightBytes);

            var response = new DiffResponse
            {
                DiffResultType = result.Type.ToString(),
                Diffs = result.Diffs?.ConvertAll(d => new DiffSegmentDto
                {
                    Offset = d.Offset,
                    Length = d.Length
                })
            };

            return Ok(response);
        }
        catch (FormatException)
        {
            // Invalid base64
            return BadRequest();
        }
    }
}
