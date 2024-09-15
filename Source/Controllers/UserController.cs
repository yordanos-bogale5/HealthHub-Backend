using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Auth0.AspNetCore.Authentication.BackchannelLogout;
using FluentValidation;
using FluentValidation.Results;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace HealthHub.Source.Controllers;

/// <summary>
/// User Controller handles routes related to a user from the client.
/// </summary>
/// <param name="userService"></param>
/// <param name="patientService"></param>
/// <param name="logger"></param>
/// <param name="appConfig"></param>
/// <param name="registerUserValidator"></param>
/// <param name="editProfileValidator"></param>
[ApiController]
[Route("api/users")]
public class UserController(
  UserService userService,
  PatientService patientService,
  ILogger<UserController> logger,
  AppConfig appConfig,
  IValidator<RegisterUserDto> registerUserValidator,
  IValidator<EditProfileDto> editProfileValidator
) : ControllerBase
{
  /// <summary>
  /// User registration endpoint.
  /// </summary>
  /// <param name="registerUserDto"></param>
  /// <returns>The UserId of the newly created user</returns>
  /// <exception cref="Exception"></exception>
  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      // Role based validation of payload
      var validation = registerUserValidator.Validate(registerUserDto);

      if (!validation.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.FluentValidationErrors] =
          validation.ToFluentValidationErrorResult();
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await userService.RegisterUser(registerUserDto);

      if (!response.Success)
      {
        throw new BadHttpRequestException(response.Message!);
      }

      return Ok(response.Data);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Register User\n\n");
      throw;
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

      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = appConfig.IsProduction ?? false,
        SameSite = SameSiteMode.None,
        Expires = DateTime.Now.AddSeconds(response.Data.ExpiresIn)
      };

      Response.Cookies.Append(
        AuthDefaults.AccessToken.ToSnakeCase(),
        response.Data.AccessToken,
        cookieOptions
      );

      var profile = response.Data.Auth0ProfileDto;

      foreach (
        var field in profile
          .GetType()
          .GetProperties(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
          )
      )
      {
        var value = field.GetValue(profile);
        Response.Cookies.Append(field.Name.ToSnakeCase(), value?.ToString() ?? "", cookieOptions);
      }

      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Login User");
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
      return StatusCode(204, response);
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
  [HttpGet("profile/{userId}")]
  public async Task<IActionResult> GetUserProfile(Guid userId)
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

  /// <summary>
  /// Endpoint responsible for getting the profile of the currently logged in user
  /// </summary>
  /// <returns></returns>
  [HttpGet("profile/me")]
  [Authorize]
  public async Task<IActionResult> GetMyProfile()
  {
    try
    {
      bool validGuid = Guid.TryParse(
        HttpContext.Request.Cookies[AuthDefaults.User.UserId]?.ToString(),
        out var userId
      );
      if (!validGuid)
        throw new UnauthorizedAccessException($"Please login again. Cookie is corrupt. {userId}");

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

  /// <summary>
  /// Edit the profile information of the user with the given id
  /// </summary>
  /// <param name="editProfileDto"></param>
  /// <returns></returns>
  [HttpPatch("profile")]
  public async Task<IActionResult> EditProfile([FromBody] EditProfileDto editProfileDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var validation = await editProfileValidator.ValidateAsync(editProfileDto);
      if (!validation.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.FluentValidationErrors] =
          validation.ToFluentValidationErrorResult();
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await userService.EditUserProfileAsync(editProfileDto);
      return StatusCode(response.StatusCode, response);
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to update profile");
      throw;
    }
  }
}
