namespace DiffErgoSum.Controllers;

using DiffErgoSum.Controllers.Models;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides a health check endpoint for the API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a simple health check response.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new HealthResponse());
    }
}
