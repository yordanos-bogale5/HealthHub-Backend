using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class DoctorService(ApplicationContext appContext, ILogger<DoctorService> logger)
{
  public async Task<Doctor?> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
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
      throw new Exception("Failed to Create Doctor");
    }
  }

  public async Task<Doctor?> GetDoctor(Guid userId)
  {
    try
    {
      var doctor = await appContext.Doctors.SingleOrDefaultAsync(d => d.UserId == userId);
      return doctor;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured when trying to get doctor by id {ex}");
      throw;
    }
  }

}