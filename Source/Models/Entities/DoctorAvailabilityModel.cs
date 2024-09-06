using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class DoctorAvailability
{
  public Guid DoctorAvailabilityId { get; set; } = Guid.NewGuid();

  [Required]
  public Guid DoctorId { get; set; } // <<FK>>

  [Required]
  public DayOfWeek AvailableDay { get; set; }
  public TimeOnly StartTime { get; set; } = TimeOnly.MinValue.AddHours(6);
  public TimeOnly EndTime { get; set; } = TimeOnly.MaxValue.AddHours(-6);

  public virtual required Doctor Doctor { get; set; }

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
