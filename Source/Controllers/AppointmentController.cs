// using HealthHub.Source.Config;
// using HealthHub.Source.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace HealthHub.Source.Controllers;

// [ApiController]
// [Route("api/appointments")]
// public class AppointmentController(
//     UserService userService,
//     DoctorService doctorService,
//     ILogger<UserController> logger,
//     AppConfig appConfig,
// ) : ControllerBase {

//     [HttpPost("book")]
//     public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
//     {
//         try
//         {
//             if (!ModelState.IsValid)
//             {
//                 HttpContext.Items[ErrorConstants.ModelStateErrors] = ModelState;
//                 throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
//             }

//             var response = await userService.BookAppointment(createAppointmentDto);

//             if (!response.Success)
//             {
//                 HttpContext.Items[ErrorConstants.ModelStateErrors] = response.Errors;
//                 throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
//             }

//             return Ok(response);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error occurred while booking appointment");
//             return StatusCode(StatusCodes.Status500InternalServerError, ErrorConstants.InternalServerError);
//         }
//     }

//     [HttpGet("doctor/{doctorId}")]
//     public async Task<IActionResult> GetDoctorAppointments(Guid doctorId)
//     {
//         try
//         {
//             var response = await doctorService.GetDoctorAppointments(doctorId);

//             if (!response.Success)
//             {
//                 HttpContext.Items[ErrorConstants.ModelStateErrors] = response.Errors;
//                 throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
//             }

//             return Ok(response);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error occurred while fetching doctor appointments");
//             return StatusCode(StatusCodes.Status500InternalServerError, ErrorConstants.InternalServerError);
//         }
//     }

//     [HttpGet("patient/{patientId}")]
//     public async Task<IActionResult> GetPatientAppointments(Guid patientId)
//     {
//         try
//         {
//             var response = await userService.GetPatientAppointments(patientId);

//             if (!response.Success)
//             {
//                 HttpContext.Items[ErrorConstants.ModelStateErrors] = response.Errors;
//                 throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
//             }

//             return Ok(response);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error occurred while fetching patient appointments");
//             return StatusCode(StatusCodes.Status500InternalServerError, ErrorConstants.InternalServerError);
//         }
//     }

//     [HttpDelete("{appointmentId}")]
//     public async Task<IActionResult> CancelAppointment(Guid appointmentId)
//     {
//         try
//         {
//             var response = await userService.CancelAppointment(appointmentId);

//             if (!response.Success)
//             {
//                 HttpContext.Items[ErrorConstants.ModelStateErrors] = response.Errors;
//                 throw new BadHttpRequestException(ErrorConstants.ModelValidationError);
//             }

//             return Ok(response);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error occurred while cancelling appointment");
//             return StatusCode(StatusCodes.Status500InternalServerError, ErrorConstants.InternalServerError);
//         }
//     }
// }
