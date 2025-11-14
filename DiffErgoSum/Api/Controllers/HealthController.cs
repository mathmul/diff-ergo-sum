namespace DiffErgoSum.Api.Controllers;

using DiffErgoSum.Api.Models;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides a health check endpoint for the API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _config;

    public HealthController(IConfiguration config) => _config = config;

    /// <summary>
    /// Returns a simple health check response.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get() =>
        Ok(new HealthResponse());

#if DEBUG
    /// <summary>
    /// Returns environment information for debugging purposes.
    /// </summary>
    [HttpGet("env")]
    public IActionResult GetEnv([FromServices] IServiceProvider sp)
    {
        var configDriver = _config["DB_DRIVER"] ?? "inmemory";

        var provider = sp.GetService<Infrastructure.DiffDbContext>()?.Database.ProviderName;

        return Ok(new { dbDriver = configDriver, efProvider = provider });
    }
#endif
}
