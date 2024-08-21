using FluentValidation;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Services;
using HealthHub.Source.Validation.AppointmentValidation;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController(
  AppointmentService appointmentService,
  IValidator<CreateAppointmentDto> createAppointmentDtoValidator
) : ControllerBase
{
  /// <summary>
  /// Allows booking of appointment for patients and doctors
  /// </summary>
  /// <param name="createAppointmentDto"></param>
  /// <returns></returns>
  [HttpPost("book")]
  public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentDto createAppointmentDto
  )
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var fluentValidation = createAppointmentDtoValidator.Validate(createAppointmentDto);

      if (!fluentValidation.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.FluentValidationErrors] =
          fluentValidation.ToFluentValidationErrorResult();
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await appointmentService.CreateAppointmentAsync(createAppointmentDto);
      if (!response.Success)
        throw new BadHttpRequestException(response.Message!);
      return Ok(response);
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }
}
