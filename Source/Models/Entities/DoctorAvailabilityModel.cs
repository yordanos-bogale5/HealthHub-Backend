using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class DoctorAvailability
{
  public Guid DoctorAvailabilityId { get; set; } = Guid.NewGuid();

  [Required]
  public required Guid DoctorId { get; set; } // <<FK>>

  [Required]
  public required DayOfWeek AvailableDay { get; set; }
  public required TimeOnly StartTime { get; set; } = TimeOnly.MinValue.AddHours(6);
  public required TimeOnly EndTime { get; set; } = TimeOnly.MaxValue.AddHours(-6);

  public virtual required Doctor Doctor { get; set; }
}
