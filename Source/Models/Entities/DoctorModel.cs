using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Doctor
{
  public Guid DoctorId { get; set; }

  [Required]
  public Guid UserId { get; set; } // <<FK>>

  [Required]
  public required string Qualifications { get; set; }

  [Required]
  public required string Biography { get; set; }
  public DoctorStatus DoctorStatus { get; set; } = DoctorStatus.Active;

  // Doctor will be verified by staff, by default it is false
  public bool Verified { get; set; } = false;

  public virtual required User User { get; set; } // <<NAV>>

  public virtual ICollection<DoctorSpeciality> DoctorSpecialities { get; set; } =
    new HashSet<DoctorSpeciality>();

  public virtual ICollection<DoctorAvailability> DoctorAvailabilities { get; set; } =
    new HashSet<DoctorAvailability>();

  public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
}
