using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Services;

public class DoctorSpecialityService(
    ILogger<DoctorSpecialityService> logger,
    ApplicationContext appContext
) {
  public async Task<DoctorSpeciality?> CreateDoctorSpecialityAsync(
      CreateDoctorSpecialityDto doctorSpecialityDto
  ) {
    try {
      var doctorSpeciality = await appContext.DoctorSpecialities.AddAsync(
          doctorSpecialityDto.ToDoctorSpeciality()
      );
      await appContext.SaveChangesAsync();
      return doctorSpeciality.Entity;
    } catch (System.Exception ex) {
      logger.LogError(ex, "Error creating Doctor Speciality!");
      throw new Exception("Error creating Doctor Speciality!");
    }
  }

  public async Task<List<DoctorSpeciality>?> CreateDoctorSpecialitiesAsync(
      List<CreateDoctorSpecialityDto> doctorSpecialityDtos
  ) {
    try {
      List<DoctorSpeciality> createResult = [];
      foreach (CreateDoctorSpecialityDto doctorSpecialityDto in doctorSpecialityDtos) {
        var doctorSpecialityResult = await CreateDoctorSpecialityAsync(doctorSpecialityDto);
        if (doctorSpecialityResult != null) {
          createResult.Add(doctorSpecialityResult);
        }
      }
      return createResult;
    } catch (System.Exception ex) {
      logger.LogError($"Error Creating Doctor Specialities {ex}");
      throw;
    }
  }
}
