using HealthHub.Source.Data;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

public class SpecialityService(ApplicationContext appContext, ILogger<SpecialityService> logger)
{
  public async Task<Speciality?> CreateSpeciality(SpecialityDto specialityDto)
  {
    try
    {

      // TODO

      return null;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error creating Speciality!");
      throw;
    }
  }
}