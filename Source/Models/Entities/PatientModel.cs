namespace HealthHub.Source.Models.Entities;

public class Patient
{
  public Guid PatientId { get; set; }
  public string? MedicalHistory { get; set; }
  public string? EmergencyContactName { get; set; }
  public string? EmergencyContactPhone { get; set; }
}
