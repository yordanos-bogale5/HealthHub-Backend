using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/doctors")]
public class DoctorController(DoctorService doctorService, ILogger<DoctorController> logger)
  : ControllerBase
{
  /// <summary>
  /// Get all Doctors from the database
  /// </summary>
  /// <returns>An array of Doctor Users</returns>
  /// <exception cref="Exception"></exception>
  [HttpGet("all")]
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
  [HttpGet("speciality/{specialityName}")]
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
  [HttpGet("name/{doctorName}")]
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
  /// Gets the available days for a doctor along with the times they are available at for that day
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns></returns>
  [HttpGet("availabilities/{doctorId}")]
  public async Task<IActionResult> GetDoctorAvailabilities([FromRoute] Guid doctorId)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await doctorService.GetDoctorAvailabilitiesAsync(doctorId);
      return StatusCode(response.StatusCode, response);
    }
    catch (System.Exception)
    {
      throw;
    }
  }

  /// <summary>
  /// Gets all available days for a doctor
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns></returns>
  [HttpGet("availabilities/days/{doctorId}")]
  public async Task<IActionResult> GetDoctorAvailableDays([FromRoute] Guid doctorId)
  {
    try
    {
      return StatusCode(200);
    }
    catch (System.Exception)
    {
      throw;
    }
  }

  /// <summary>
  /// Gets all available times for a doctor on a specific day
  /// </summary>
  /// <param name="doctorId"></param>
  /// <param name="day"></param>
  /// <returns></returns>
  [HttpGet("availabilities/{doctorId}/day/{day}/")]
  public async Task<IActionResult> GetDoctorAvailableTimesForDay(
    [FromRoute] [Required] Guid doctorId,
    [FromRoute] [Required] Guid day
  )
  {
    try
    {
      return StatusCode(200);
    }
    catch (System.Exception)
    {
      throw;
    }
  }
}