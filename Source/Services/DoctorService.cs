using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

public class DoctorService(ApplicationContext appContext, ILogger<DoctorService> logger)
{
  public async Task<Doctor?> CreateDoctor(CreateDoctorDto createDoctorDto)
  {
    try
    {
      var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
      var doctor = doctorResult.Entity;

      await appContext.SaveChangesAsync();
      return doctor;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Doctor");
      throw;
    }
  }
}