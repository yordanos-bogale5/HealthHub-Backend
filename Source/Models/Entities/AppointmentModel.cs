using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Entities;

public class Appointment
{
  public Guid AppointmentId { get; set; }

  [Required]
  public Guid DoctorId { get; set; } // <<FK>>

  [Required]
  public Guid PatientId { get; set; } // <<FK>>

  [Required]
  public DateTime AppointmentDate { get; set; }
  [Required]
  public TimeOnly AppointmentTime { get; set; }

  [Required]
  public AppointmentType AppointmentType { get; set; }
  public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

  public virtual Doctor? Doctor { get; set; } // <<NAV>>
  public virtual Patient? Patient { get; set; } // <<NAV>>

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}