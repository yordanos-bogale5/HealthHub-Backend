using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class DoctorService(ApplicationContext appContext, ILogger<DoctorService> logger)
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
        .Select(d => d.ToDoctorDto())
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
        .Select(d => d.ToDoctorDto())
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
        .Select(d => d.ToDoctorDto())
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
        .Select(d => d.ToDoctorDto())
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
}
