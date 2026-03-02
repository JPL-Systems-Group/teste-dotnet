using Microsoft.AspNetCore.Mvc;

namespace Orders.Api.Controllers;

[ApiController]
public sealed class HealthController : ControllerBase
{
    [HttpGet("health/live")]
    public IActionResult GetLiveness() => Ok(new { status = "alive" });
}
