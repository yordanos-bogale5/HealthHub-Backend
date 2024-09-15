using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/patients")]
public class PatientController(PatientService patientService, ILogger<PatientController> logger)
  : ControllerBase
{
  /// <summary>
  /// Retrieves all patients from the database
  /// </summary>
  /// <returns></returns>
  [HttpGet("all")]
  public async Task<IActionResult> GetAllPatients()
  {
    try
    {
      var response = await patientService.GetAllPatientsAsync();
      if (!response.Success)
        throw new Exception(response.Message);
      return Ok(response);
    }
    catch (Exception ex)
    {
      logger.LogError($"Failed to get all patients: {ex}");
      throw;
    }
  }
}
