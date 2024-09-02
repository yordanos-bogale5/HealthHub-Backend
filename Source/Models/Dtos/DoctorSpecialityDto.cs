using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Models.Dtos;

public record CreateDoctorSpecialityDto
{
  public required Guid DoctorId;
  public required Guid SpecialityId;
}
