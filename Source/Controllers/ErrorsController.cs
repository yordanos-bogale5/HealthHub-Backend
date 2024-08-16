using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Controllers;

public class ErrorsController : ControllerBase
{
  [Route("/error")]
  public IActionResult Error()
  {
    return Problem();
  }
}