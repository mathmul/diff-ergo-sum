using Microsoft.AspNetCore.Mvc;

namespace DiffErgoSum.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { ok = true });
        }
    }
}
