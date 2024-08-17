namespace HealthHub.Source.Models.Dtos;

public record CreateSpecialityDto
{
  public required string SpecialityName { get; set; }
  public required Guid DoctorId { get; set; }
}