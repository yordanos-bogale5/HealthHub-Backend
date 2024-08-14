namespace HealthHub.Source.Models.Dtos;

public record CreatePatientDto
{
  public required Guid UserId { get; init; }
  public string? MedicalHistory { get; init; }
  public string? EmergencyContactName { get; init; }
  public string? EmergencyContactPhone { get; init; }
}