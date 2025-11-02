using DiffErgoSum.Controllers.Models;

using Microsoft.AspNetCore.Mvc;

namespace DiffErgoSum.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new HealthResponse());
        }
    }
}
