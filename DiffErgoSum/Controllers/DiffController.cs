namespace DiffErgoSum.Controllers;

using DiffErgoSum.Application;
using DiffErgoSum.Controllers.Models;
using DiffErgoSum.Domain;

using Microsoft.AspNetCore.Mvc;


/// <summary>
/// Provides endpoints for submitting Base64-encoded data and retrieving their differences.
/// </summary>
/// <remarks>
/// Implements the main diffing API:
/// <list type="bullet">
/// <item><description><c>PUT /api/v1/diff/{id}/left</c> – upload the left side</description></item>
/// <item><description><c>PUT /api/v1/diff/{id}/right</c> – upload the right side</description></item>
/// <item><description><c>GET /api/v1/diff/{id}</c> – compare both sides and return a diff result</description></item>
/// </list>
/// Uses an in-memory repository for temporary storage during testing.
/// </remarks>
[ApiController]
[Route("api/v1/diff/{id}")]
public class DiffController : ControllerBase
{
    private readonly IDiffRepository _repo;
    private readonly IDiffService _service;

    public DiffController(IDiffRepository repo, IDiffService service)
    {
        _repo = repo;
        _service = service;
    }

    /// <summary>
    /// Uploads the left side of the diff pair.
    /// </summary>
    /// <param name="id">The diff identifier.</param>
    /// <param name="request">The request body containing the Base64-encoded data.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><see cref="StatusCodes.Status201Created"/> if successfully stored.</description></item>
    /// <item><description><see cref="StatusCodes.Status400BadRequest"/> if model validation fails (missing or malformed JSON).</description></item>
    /// <item><description><see cref="StatusCodes.Status422UnprocessableEntity"/> if <c>data</c> is syntactically valid but not decodable Base64.</description></item>
    /// </list>
    /// </returns>
    [HttpPut("left")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult UploadLeft(int id, [FromBody] DiffRequest request)
    {
        if (!IsBase64(request.Data))
            return StatusCode(StatusCodes.Status422UnprocessableEntity, new { error = "InvalidBase64", message = "Provided data is not valid Base64." });

        _repo.SaveLeft(id, request.Data);
        return Created(string.Empty, null);
    }

    /// <summary>
    /// Uploads the right side of the diff pair.
    /// </summary>
    /// <param name="id">The diff identifier.</param>
    /// <param name="request">The request body containing the Base64-encoded data.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><see cref="StatusCodes.Status201Created"/> if successfully stored.</description></item>
    /// <item><description><see cref="StatusCodes.Status400BadRequest"/> if model validation fails (missing or malformed JSON).</description></item>
    /// <item><description><see cref="StatusCodes.Status422UnprocessableEntity"/> if <c>data</c> is syntactically valid but not decodable Base64.</description></item>
    /// </list>
    /// </returns>
    [HttpPut("right")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult UploadRight(int id, [FromBody] DiffRequest request)
    {
        if (!IsBase64(request.Data))
            return StatusCode(StatusCodes.Status422UnprocessableEntity, new { error = "InvalidBase64", message = "Provided data is not valid Base64." });

        _repo.SaveRight(id, request.Data);
        return Created(string.Empty, null);
    }

    /// <summary>
    /// Compares the uploaded left and right Base64 payloads and returns their differences.
    /// </summary>
    /// <param name="id">The diff identifier corresponding to the uploaded pair.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><see cref="StatusCodes.Status200OK"/> with a <see cref="DiffResponse"/> body if comparison succeeds.</description></item>
    /// <item><description><see cref="StatusCodes.Status404NotFound"/> if one or both sides are missing.</description></item>
    /// <item><description><see cref="StatusCodes.Status400BadRequest"/> if either stored value is not valid Base64.</description></item>
    /// </list>
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(DiffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetDiff(int id)
    {
        var pair = _repo.Get(id);
        if (pair == null || string.IsNullOrEmpty(pair.Value.Left) || string.IsNullOrEmpty(pair.Value.Right))
            return NotFound();

        try
        {
            var leftBytes = Convert.FromBase64String(pair.Value.Left);
            var rightBytes = Convert.FromBase64String(pair.Value.Right);

            var result = _service.Compare(leftBytes, rightBytes);

            var response = new DiffResponse(
                result.Type.ToString(),
                result.Diffs?.ConvertAll(d => new DiffResponseSegmentDto(d.Offset, d.Length))
            );

            return Ok(response);
        }
        catch (FormatException)
        {
            // Invalid base64
            return BadRequest();
        }
    }

    /// <summary>
    /// Validates whether a string is a valid Base64 value.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns><see langword="true"/> if valid Base64; otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an unexpected error occurs during conversion.
    /// </exception>
    private static bool IsBase64(string value)
    {
        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Unexpected error during Base64 validation.", e);
        }
    }
}
