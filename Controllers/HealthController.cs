using Microsoft.AspNetCore.Mvc;

namespace Mcube_api.Controllers;

[Route("health")]
public class HealthController : ControllerBase
{
    public HealthController()
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
