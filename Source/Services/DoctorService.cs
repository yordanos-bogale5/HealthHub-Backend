using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class DoctorService(
  ApplicationContext appContext,
  SpecialityService specialityService,
  DoctorSpecialityService doctorSpecialityService,
  Lazy<AvailabilityService> availabilityService,
  ILogger<DoctorService> logger
)
{
  public async Task<Doctor?> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
  {
    try
    {
      var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
      var doctor = doctorResult.Entity;
      logger.LogInformation($"\nCreate Doctor Result: \n {doctorResult}");
      await appContext.SaveChangesAsync();
      return doctor;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Doctor");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetAllDoctors()
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(true, 200, doctorUsers, "All Doctors Retrieved!");
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get all Doctors in User Service: {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsBySpecialityAsync(
    string specialityName
  )
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(ds.Speciality.SpecialityName, $"%{specialityName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with speciality name retrieved"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get doctors by speciality in doctor service {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByNameAsync(string doctorName)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(d.User.FirstName + " " + d.User.LastName, $"%{doctorName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with name retrieved"
      );
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByGenderAsync(Gender gender)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d => d.User.Gender == gender)
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with gender retrieved"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Getting doctors by gender failed: {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<Doctor>> GetDoctorAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .SingleOrDefaultAsync(d => d.DoctorId == doctorId);

      if (doctor == null)
      {
        return new ServiceResponse<Doctor>(false, 404, null, "Doctor not found");
      }

      return new ServiceResponse<Doctor>(true, 200, doctor, "Doctor found");
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get doctor by id {ex}");
      throw;
    }
  }

  /// <summary>
  /// Checks if a doctor exists
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>True if doctor exists, otherwise False </returns>
  public async Task<bool> CheckDoctorExistsAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext.Doctors.FindAsync(doctorId);
      return doctor != null;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to check if doctor exists {ex}");
      throw;
    }
  }

  public async Task<DoctorDto> EditDoctorAsync(EditDoctorProfileDto editDoctorProfileDto)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .FirstOrDefaultAsync(d => d.UserId == editDoctorProfileDto.UserId);

      if (doctor == null)
      {
        throw new BadHttpRequestException("Doctor with that userId is not present.");
      }

      /* Creating the New Specialtities */
      var specialities =
        editDoctorProfileDto.Specialitites != null
          ? await specialityService.CreateSpecialitiesAsync(
            editDoctorProfileDto.Specialitites.ToSpecialityList(doctor.DoctorId)
          )
          : null;

      // Create doctor specialities based on the new specialities
      var docSpecs =
        specialities != null
          ? await doctorSpecialityService.CreateDoctorSpecialitiesAsync(
            specialities.Select(s => s.ToCreateDoctorSpecialityDto(doctor)).ToList()
          )
          : null;

      /* Creating the New Doctor Availabilities */
      var availabilitiesResponse =
        editDoctorProfileDto.Availabilities != null
          ? await availabilityService.Value.AddDoctorAvailabilityAsync(
            editDoctorProfileDto.Availabilities,
            doctor
          )
          : null;

      /* Perform Updates */
      doctor.DoctorSpecialities = docSpecs ?? doctor.DoctorSpecialities;
      doctor.Qualifications = editDoctorProfileDto.Qualifications ?? doctor.Qualifications;
      doctor.DoctorStatus = editDoctorProfileDto.DoctorStatus ?? doctor.DoctorStatus;
      doctor.Biography = editDoctorProfileDto.Biography ?? doctor.Biography;
      doctor.DoctorAvailabilities =
        availabilitiesResponse != null
          ? availabilitiesResponse.Data ?? doctor.DoctorAvailabilities
          : doctor.DoctorAvailabilities;

      await appContext.SaveChangesAsync();

      return doctor.ToDoctorDto(doctor.User, doctor.DoctorSpecialities);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to edit doctor.");
      throw;
    }
  }
}
