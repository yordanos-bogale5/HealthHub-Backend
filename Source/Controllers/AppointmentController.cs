using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController(
    UserService userService,
    DoctorService doctorService,
    AppointmentService appointmentService,
    ILogger<UserController> logger,
    AppConfig appConfig
) : ControllerBase {
  [HttpPost("create")]
  public async Task<IActionResult> CreateAppointment(
      [FromBody] CreateAppointmentDto createAppointmentDto
  ) {
    try {
      if (!ModelState.IsValid) {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await appointmentService.CreateAppointmentAsync(createAppointmentDto);
      if (!response.Success)
        throw new BadHttpRequestException(response.Message!);
      return Ok(response);
    } catch (System.Exception ex) {
      throw;
    }
  }
}
