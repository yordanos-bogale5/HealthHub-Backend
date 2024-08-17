using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

public class SpecialityService(ApplicationContext appContext, ILogger<SpecialityService> logger)
{
  public async Task<Speciality?> CreateSpecialityAsync(CreateSpecialityDto specialityDto)
  {
    try
    {
      var speciality = await appContext.Specialities.AddAsync(specialityDto.ToSpeciality());
      await appContext.SaveChangesAsync();
      return speciality.Entity;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error creating Speciality!");
      throw new Exception("Error creating Speciality!");
    }
  }

  public async Task<List<Speciality>?> CreateSpecialitiesAsync(List<CreateSpecialityDto> specialityDtos)
  {
    try
    {
      List<Speciality> createResult = [];
      foreach (CreateSpecialityDto specialityDto in specialityDtos)
      {
        var specialityResult = await CreateSpecialityAsync(specialityDto);
        if (specialityResult != null)
        {
          createResult.Add(specialityResult);
        }
      }
      return createResult;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Error Creating Specialities {ex}");
      throw;
    }
  }
}