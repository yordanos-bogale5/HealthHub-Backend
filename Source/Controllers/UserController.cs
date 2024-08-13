using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Services;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Config;
using Microsoft.AspNetCore.Authorization;

namespace HealthHub.Source.Controllers;

/// <summary>
/// User Controller handles routes related to a user from the client.
/// </summary>
/// <param name="userService"></param>
/// <param name="logger"></param>
[ApiController]
[Route("api/users")]
public class UserController(UserService userService, ILogger<UserController> logger, AppConfig appConfig) : ControllerBase
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

      return Ok(response.Data);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Register User");
      throw new Exception("Internal Server Error ", ex);
    }
  }

  /// <summary>
  /// This endpoint is responsible for logging in a user.
  /// </summary>
  /// <param name="loginUserDto"></param>
  /// <returns></returns>
  [HttpPost("login")]
  public async Task<IActionResult> LoginUserAsync(LoginUserDto loginUserDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var response = await userService.LoginUserAsync(loginUserDto);

      if (!response.Success || response.Data == null)
      {
        return StatusCode(response.StatusCode, response.Message);
      }

      Response.Cookies.Append("access_token", response.Data.AccessToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = appConfig.IsProduction ?? false,
        SameSite = SameSiteMode.None,
        Expires = DateTime.Now.AddSeconds(response.Data.ExpiresIn)
      });

      return Ok(response);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to Login User");
      return StatusCode(500, ex.Message);
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

  /// <summary>
  /// This endpoint deletes the user with the specified userId.
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  [HttpDelete("{userId}")]
  public async Task<IActionResult> DeleteUser(Guid userId)
  {
    try
    {
      var response = await userService.DeleteUserAsync(userId);
      if (!response.Success)
      {
        return StatusCode(response.StatusCode, response.Message);
      }
      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Delete User");

      throw;
    }
  }

  /// <summary>
  /// This endpoint returns the profile of the user with the specified userId.
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  [HttpGet("{userId}/profile")]
  [Authorize]
  public async Task<IActionResult> Profile(Guid userId)
  {
    try
    {
      var response = await userService.GetUserProfile(userId);
      if (!response.Success)
      {
        return StatusCode(response.StatusCode, response.Message);
      }
      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to get user profile");

      throw;
    }
  }
}