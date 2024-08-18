using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Models.Dtos;

public record CreatePatientDto
{
    public required User User { get; init; }
    public string? MedicalHistory { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactPhone { get; init; }
}
