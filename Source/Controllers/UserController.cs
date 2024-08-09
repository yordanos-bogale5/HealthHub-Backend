using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Services;
using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{

  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      // Make Service Invocation
      var response = await userService.RegisterUser(registerUserDto);

      if (!response.Success)
      {
        return StatusCode(response.StatusCode, response.Message);
      }

      // Successful Registration
      var result = new
      {
        userId = response.Data
      };

      return Ok(result);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw new Exception("Internal Server Error ", ex);
    }
  }

  [HttpGet("all")]
  public async Task<IActionResult> GetAllUsers()
  {
    try
    {
      var response = await userService.GetAllUsers();
      if (!response.Success)
      {
        return StatusCode(response.StatusCode, response.Message);
      }
      return Ok(response);
    }
    catch (Exception)
    {
      throw;
    }
  }
}