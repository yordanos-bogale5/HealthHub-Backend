using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Services;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{

  [HttpPost]
  [Route("register")]
  public IActionResult RegisterUser()
  {
    return Ok("User Registered");
  }

  [HttpGet]
  [Route("all")]
  public IActionResult GetAllUsers()
  {
    return Ok(userService.GetAllUsers());
  }
}