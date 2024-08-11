using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Services;
using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Controllers;

/// <summary>
/// User Controller handles routes related to a user from the client.
/// </summary>
/// <param name="userService"></param>
[ApiController]
[Route("api/users")]
public class UserController(UserService userService) : ControllerBase
{

  /// <summary>
  /// User registration endpoint.
  /// </summary>
  /// <param name="registerUserDto"></param>
  /// <returns>The UserId of the newly created user</returns>
  /// <exception cref="Exception"></exception>
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

  [HttpPost("login")]
  public async Task<IActionResult> LoginUser(LoginUserDto loginUserDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      // var response = await userService.LoginUser(loginUserDto);

      // if (!response.Success)
      // {
      //   return StatusCode(response.StatusCode, response.Message);
      // }

      // return Ok(response);
      return Ok();
    }
    catch (System.Exception ex)
    {
      Console.WriteLine($"{ex}");
      throw;
    }
  }

  /// <summary>
  /// This endpoint returns all the users in the database.
  /// </summary>
  /// <returns>List of <see cref="UserDto"/></returns>
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