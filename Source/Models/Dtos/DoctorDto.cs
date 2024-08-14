using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Dtos;

public record CreateDoctorDto
{
  public required Guid UserId { get; init; }
  public required Guid SpecialityId { get; init; }
  public required string Qualifications { get; init; }
  public required string Biography { get; init; }
  public DoctorStatus DoctorStatus { get; init; } = DoctorStatus.Active;
}