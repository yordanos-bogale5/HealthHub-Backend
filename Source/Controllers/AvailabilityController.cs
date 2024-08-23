using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(AvailabilityService availabilityService) : ControllerBase
{
  /// <summary>
  /// Gets the available days for a doctor along with the times they are available at for that day
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns></returns>
  [HttpGet("doctor/{doctorId}")]
  public async Task<IActionResult> GetDoctorAvailabilities([FromRoute] Guid doctorId)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await availabilityService.GetDoctorAvailabilitiesAsync(doctorId);
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
  [HttpGet("doctor/{doctorId}/days")]
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
  /// <returns></returns>
  [HttpGet("doctors/{doctorId}/times/{day}")]
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
