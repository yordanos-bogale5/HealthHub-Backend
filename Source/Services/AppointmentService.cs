using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Responses;

namespace HealthHub.Source.Services;

public class AppointmentService()
{
  public async Task<ServiceResponse<AppointmentDto>> CreateAppointmentAsync(
    CreateAppointmentDto createAppointmentDto
  )
  {
    try
    {
      return new ServiceResponse<AppointmentDto>
      {
        Data = null,
        Message = "Appointment created successfully",
      };
    }
    catch (System.Exception ex)
    {
      throw new Exception("An error occured when trying to create an appointment");
    }
  }
}
