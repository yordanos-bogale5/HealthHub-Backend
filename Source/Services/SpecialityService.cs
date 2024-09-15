using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class SpecialityService(ApplicationContext appContext, ILogger<SpecialityService> logger)
{
  public async Task<Speciality?> CreateSpecialityAsync(CreateSpecialityDto specialityDto)
  {
    try
    {
      var existentSpeciality = await appContext.Specialities.FirstOrDefaultAsync(s =>
        s.SpecialityName == specialityDto.SpecialityName
      );

      // If Speciality with that name already exists then there is no need to add one, just return
      if (existentSpeciality != null)
      {
        return existentSpeciality;
      }

      var speciality = await appContext.Specialities.AddAsync(specialityDto.ToSpeciality());
      await appContext.SaveChangesAsync();
      return speciality.Entity;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error creating Speciality!");
      throw;
    }
  }

  public async Task<List<Speciality>> CreateSpecialitiesAsync(
    List<CreateSpecialityDto> specialityDtos
  )
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
    catch (Exception ex)
    {
      logger.LogError($"Error Creating Specialities {ex}");
      throw;
    }
  }
}
