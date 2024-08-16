using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

public class PatientService(ILogger<PatientService> logger, ApplicationContext appContext)
{
  public async Task<Patient?> CreatePatientAsync(CreatePatientDto createPatientDto)
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
      throw;
    }
  }
}