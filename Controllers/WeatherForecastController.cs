using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleController : ControllerBase
{
  [HttpGet]
  public IActionResult Get()
  {
    return Ok("Hello World");
  }
}
