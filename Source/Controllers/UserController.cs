using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Dtos;
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
/// <param name="logger"></param>
/// <param name="appConfig"></param>
/// <param name="registerUserValidator"></param>
[ApiController]
[Route("api/users")]
public class UserController(
    UserService userService,
    DoctorService doctorService,
    ILogger<UserController> logger,
    AppConfig appConfig,
    IValidator<RegisterUserDto> registerUserValidator
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
                HttpContext.Items[ErrorConstants.ModelStateErrors] = ModelState;
                throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
            }

            // Role based validation of payload
            var validation = registerUserValidator.Validate(registerUserDto);

            if (!validation.IsValid)
            {
                HttpContext.Items[ErrorConstants.FluentValidationErrors] =
                    validation.ToFluentValidationErrorResult();
                throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
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
            logger.LogError(ex, "Failed to Register User");
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

            Response.Cookies.Append(
                "access_token",
                response.Data.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = appConfig.IsProduction ?? false,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddSeconds(response.Data.ExpiresIn)
                }
            );

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

    /// <summary>
    /// Get all Doctors from the database
    /// </summary>
    /// <returns>An array of Doctor Users</returns>
    /// <exception cref="Exception"></exception>
    [HttpGet("doctors/all")]
    public async Task<IActionResult> GetAllDoctors()
    {
        try
        {
            var response = await doctorService.GetAllDoctors();
            if (!response.Success)
                throw new Exception(response.Message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError($"Internal Server Error: {ex}");
            throw new Exception("Failed to get all doctors in controller.");
        }
    }

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
            throw new Exception("Error getting doctors by speciality");
        }
    }

    // [HttpGet("doctors/name/{doctorName}")]
    // public async Task<IActionResult> GetDoctorsByName(string doctorName)
    // {
    //   try
    //   {
    //     var response = await doctorService.GetDoctorsByNameAsync(doctorName);
    //     if (!response.Success) throw new Exception(response.Message);
    //     return Ok(response);
    //   }
    //   catch (Exception ex)
    //   {
    //     logger.LogError($"Internal Server Error: {ex}");
    //     throw new Exception("Error getting doctors by name");
    //   }
    // }
}
