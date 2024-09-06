using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;

public class PatientService(ILogger<PatientService> logger, ApplicationContext appContext)
{
  public async Task<Patient> CreatePatientAsync(CreatePatientDto createPatientDto)
  {
    try
    {
      var patientResult = await appContext.Patients.AddAsync(createPatientDto.ToPatient());
      var patient = patientResult.Entity;

      await appContext.SaveChangesAsync();

      return patient;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to Create Patient");
      throw new Exception("Failed to Create Patient");
    }
  }

  /// <summary>
  /// Gets all patients from the database
  /// </summary>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<List<PatientDto>>> GetAllPatientsAsync()
  {
    try
    {
      var patients = await appContext
        .Patients.Include(p => p.User)
        .Select(p => p.ToPatientDto(p.User))
        .ToListAsync();

      return new ServiceResponse<List<PatientDto>>
      {
        Data = patients,
        Message = "Fetched all patients",
        StatusCode = 200,
        Success = true
      };
    }
    catch (System.Exception ex)
    {
      throw new Exception("Failed to get all patients");
    }
  }

  public async Task<ServiceResponse<Patient>> GetPatientAsync(Guid patientId)
  {
    try
    {
      var patient = await appContext
        .Patients.Include(p => p.User)
        .SingleOrDefaultAsync(p => p.PatientId == patientId);
      if (patient == null)
      {
        return new ServiceResponse<Patient>
        {
          Data = null,
          Message = "Patient Not Found.",
          StatusCode = 404,
          Success = false
        };
      }
      return new ServiceResponse<Patient>(true, 200, patient, "Patient found.");
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to Get Patient");
      throw;
    }
  }

  public async Task<bool> CheckPatientExistsAsync(Guid patientId)
  {
    try
    {
      var patient = await appContext.Patients.FindAsync(patientId);
      return patient != null;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to check if patient exists {ex}");
      throw;
    }
  }

  public async Task<PatientProfileDto> EditPatientProfileAsync(
    EditPatientProfileDto editPatientProfileDto
  )
  {
    try
    {
      var patient = await appContext
        .Patients.Include(p => p.User)
        .FirstOrDefaultAsync(p => p.UserId == editPatientProfileDto.UserId);

      if (patient == null)
      {
        throw new BadHttpRequestException("Patient with that userId is not found.");
      }

      // Update fields where payload is not null
      patient.MedicalHistory = editPatientProfileDto.MedicalHistory ?? patient.MedicalHistory;
      patient.EmergencyContactPhone =
        editPatientProfileDto.EmergencyContactPhone ?? patient.EmergencyContactPhone;
      patient.EmergencyContactName =
        editPatientProfileDto.EmergencyContactName ?? patient.EmergencyContactName;

      await appContext.SaveChangesAsync();
      return patient.ToPatientProfileDto(patient.User);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to edit patient profile.");
      throw;
    }
  }

  /// <summary>
  /// Retrieves the profile of the patient specified with the userId
  /// </summary>
  /// <param name="userId"></param>
  /// <returns>A <see cref="PatientProfileDto"/> representing the patients's profile.</returns>
  /// <exception cref="KeyNotFoundException"/>Thrown when no patient is found with the specified userId.<exception/>
  public async Task<PatientProfileDto> GetPatientProfileAsync(Guid userId)
  {
    try
    {
      var patient = await appContext
        .Patients.Where(p => p.UserId == userId)
        .Include(p => p.User)
        .SingleOrDefaultAsync();

      if (patient == null)
      {
        throw new KeyNotFoundException(
          "Patient with that user id is not found. Couldn't retrieve profile information."
        );
      }

      return patient.ToPatientProfileDto(patient.User);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured tring to get patient profile");
      throw;
    }
  }
}
