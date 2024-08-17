using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("/api")]
public class ErrorsController : ControllerBase
{
  [HttpGet("error")]
  public IActionResult Error()
  {
    return Problem();
  }
}