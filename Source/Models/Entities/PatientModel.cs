using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Patient
{
  public Guid PatientId { get; set; }

  [Required]
  public required Guid UserId { get; set; } // <<FK>>

  public string? MedicalHistory { get; set; }
  public string? EmergencyContactName { get; set; }
  public string? EmergencyContactPhone { get; set; }

  public virtual required User User { get; set; }
  public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
  public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
