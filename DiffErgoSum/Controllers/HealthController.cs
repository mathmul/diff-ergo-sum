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
    private readonly IConfiguration _config;

    public HealthController(IConfiguration config) => _config = config;

    /// <summary>
    /// Returns a simple health check response.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get() =>
        Ok(new HealthResponse());

    // TODO: remove as it is only for debugging purposes
    [HttpGet("env")]
    public IActionResult GetEnv([FromServices] IServiceProvider sp)
    {
        var configDriver = _config["DB_DRIVER"] ?? "inmemory";

        // Ask EF what provider is actually wired
        var provider = sp.GetService<DiffErgoSum.Infrastructure.DiffDbContext>()?
                          .Database.ProviderName;

        return Ok(new { dbDriver = configDriver, efProvider = provider });
    }
}
