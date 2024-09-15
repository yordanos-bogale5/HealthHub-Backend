using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Source.Services;

public class DoctorSpecialityService(
  ILogger<DoctorSpecialityService> logger,
  ApplicationContext appContext
)
{
  public async Task<DoctorSpeciality> CreateDoctorSpecialityAsync(
    CreateDoctorSpecialityDto doctorSpecialityDto
  )
  {
    try
    {
      var existentDoctorSpeciality = await appContext.DoctorSpecialities.FirstOrDefaultAsync(ds =>
        ds.DoctorId == doctorSpecialityDto.DoctorId
        && ds.SpecialityId == doctorSpecialityDto.SpecialityId
      );

      if (existentDoctorSpeciality != null)
      {
        return existentDoctorSpeciality;
      }

      var doctorSpeciality = await appContext.DoctorSpecialities.AddAsync(
        doctorSpecialityDto.ToDoctorSpeciality(
          doctorSpecialityDto.DoctorId,
          doctorSpecialityDto.SpecialityId
        )
      );
      await appContext.SaveChangesAsync();
      return doctorSpeciality.Entity;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error creating Doctor Speciality!");
      throw;
    }
  }

  public async Task<List<DoctorSpeciality>> CreateDoctorSpecialitiesAsync(
    List<CreateDoctorSpecialityDto> doctorSpecialityDtos
  )
  {
    try
    {
      List<DoctorSpeciality> createResult = [];
      foreach (CreateDoctorSpecialityDto doctorSpecialityDto in doctorSpecialityDtos)
      {
        var doctorSpecialityResult = await CreateDoctorSpecialityAsync(doctorSpecialityDto);
        if (doctorSpecialityResult != null)
        {
          createResult.Add(doctorSpecialityResult);
        }
      }
      return createResult;
    }
    catch (Exception ex)
    {
      logger.LogError($"Error Creating Doctor Specialities {ex}");
      throw;
    }
  }
}
