using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;

public class PatientService(ILogger<PatientService> logger, ApplicationContext appContext) {
  public async Task<Patient?> CreatePatientAsync(CreatePatientDto createPatientDto) {
    try {
      var patientResult = await appContext.Patients.AddAsync(createPatientDto.ToPatient());
      var patient = patientResult.Entity;

      await appContext.SaveChangesAsync();

      return patient;
    } catch (System.Exception ex) {
      logger.LogError(ex, "Failed to Create Patient");
      throw new Exception("Failed to Create Patient");
    }
  }

  /// <summary>
  /// Gets all patients from the database
  /// </summary>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<List<PatientDto>>> GetAllPatientsAsync() {
    try {
      var patients = await appContext
          .Patients.Include(p => p.User)
          .Select(p => p.ToPatientDto())
          .ToListAsync();

      return new ServiceResponse<List<PatientDto>> {
        Data = patients,
        Message = "Fetched all patients",
        StatusCode = 200,
        Success = true
      };
    } catch (System.Exception ex) {
      throw new Exception("Failed to get all patients");
    }
  }
}
