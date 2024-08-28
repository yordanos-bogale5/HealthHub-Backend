using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public record CreatePatientDto
{
  public required User User { get; init; }
  public string? MedicalHistory { get; init; }
  public string? EmergencyContactName { get; init; }
  public string? EmergencyContactPhone { get; init; }
}

public record EditPatientProfileDto(
  Guid UserId,
  string? MedicalHistory,
  string? EmergencyContactName,
  string? EmergencyContactPhone
);

public record PatientDto
{
  public required Guid UserId { get; init; }
  public required Guid PatientId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
  public required bool IsEmailVerified { get; set; }
  public required string Phone { get; init; }
  public required Gender Gender { get; init; }
  public required DateTime DateOfBirth { get; init; }
  public required string ProfilePicture { get; set; }
  public required string Address { get; init; }

  public required string MedicalHistory { get; init; }
  public required string EmergencyContactName { get; init; }
  public required string EmergencyContactPhone { get; init; }
}
