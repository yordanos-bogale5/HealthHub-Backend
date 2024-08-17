using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Speciality
{
  public Guid SpecialityId { get; set; }
  [Required]
  public required Guid DoctorId { get; set; } // <<FK>>
  [Required]
  public required string SpecialityName { get; set; }

  public virtual Doctor? Doctor { get; set; } // Nav

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}