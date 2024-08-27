using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Auth0.AspNetCore.Authentication.BackchannelLogout;
using FluentValidation;
using FluentValidation.Results;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Constants;
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
/// <param name="doctorService"></param>
/// <param name="patientService"></param>
/// <param name="logger"></param>
/// <param name="appConfig"></param>
/// <param name="registerUserValidator"></param>
/// <param name="editProfileValidator"></param>
[ApiController]
[Route("api/users")]
public class UserController(
  UserService userService,
  DoctorService doctorService,
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
    catch (System.Exception ex)
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
  [Authorize]
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
  /// Get all Doctors from the database
  /// </summary>
  /// <returns>An array of Doctor Users</returns>
  /// <exception cref="Exception"></exception>
  [HttpGet("doctors/all")]
  public async Task<IActionResult> GetAllDoctors([FromQuery] Gender? gender = null)
  {
    try
    {
      IServiceResponse response;
      if (gender != null)
      {
        response = await doctorService.GetDoctorsByGenderAsync((Gender)gender);
      }
      else
        response = await doctorService.GetAllDoctors();

      if (!response.Success)
        throw new Exception(response.Message);

      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError($"Internal Server Error: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Get all doctors with the specified speciality
  /// </summary>
  /// <param name="specialityName"></param>
  /// <returns></returns>
  [HttpGet("doctors/speciality/{specialityName}")]
  public async Task<IActionResult> GetDoctorsBySpeciality(string specialityName)
  {
    try
    {
      var response = await doctorService.GetDoctorsBySpecialityAsync(specialityName);
      if (!response.Success)
        throw new Exception(response.Message);
      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError($"Internal Server Error: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Get all doctors with the specified name
  /// </summary>
  /// <param name="doctorName"></param>
  /// <returns></returns>
  /// <exception cref="BadHttpRequestException"></exception>
  [HttpGet("doctors/name/{doctorName}")]
  public async Task<IActionResult> GetDoctorsByName(
    [FromRoute] [Required] [MinLength(1)] string doctorName
  )
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }
      var response = await doctorService.GetDoctorsByNameAsync(doctorName);
      if (!response.Success)
        throw new Exception(response.Message);
      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError($"Internal Server Error: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Retrieves all patients from the database
  /// </summary>
  /// <returns></returns>
  [HttpGet("patients/all")]
  public async Task<IActionResult> GetAllPatients()
  {
    try
    {
      var response = await patientService.GetAllPatientsAsync();
      if (!response.Success)
        throw new Exception(response.Message);
      return Ok(response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get all patients: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Edit the profile information of the user with the given id
  /// </summary>
  /// <param name="editProfileDto"></param>
  /// <param name="userId"></param>
  /// <returns></returns>
  [HttpPatch("profile/{userId}")]
  public async Task<IActionResult> EditProfile(
    [FromBody] EditProfileDto editProfileDto,
    [FromRoute] Guid userId
  )
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var validation = editProfileValidator.Validate(editProfileDto);
      if (!validation.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.FluentValidationErrors] =
          validation.ToFluentValidationErrorResult();
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await userService.EditUserProfileAsync(editProfileDto, userId);
      return StatusCode(response.StatusCode, response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to update profile");
      throw;
    }
  }
}
