using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Doctor
{
  public Guid DoctorId { get; set; } = Guid.NewGuid();

  [Required]
  public required Guid UserId { get; set; } // <<FK>>

  [Required]
  public required string Qualifications { get; set; }

  [Required]
  public required string Biography { get; set; }
  public DoctorStatus DoctorStatus { get; set; } = DoctorStatus.Active;

  // Doctor will be verified by staff, by default it is false
  public bool IsVerified { get; set; } = false;

  public required Guid DoctorPreferenceId { get; set; }
  public virtual DoctorPreference? DoctorPreference { get; set; }

  public required Guid CvId { get; set; }
  public virtual File? Cv { get; set; }

  public virtual required User User { get; set; } // <<NAV>>

  public virtual ICollection<DoctorSpeciality> DoctorSpecialities { get; set; } =
    new HashSet<DoctorSpeciality>();
  public virtual ICollection<DoctorAvailability> DoctorAvailabilities { get; set; } =
    new HashSet<DoctorAvailability>();

  public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();

  public virtual ICollection<Education> Educations { get; set; } = new HashSet<Education>();
  public virtual ICollection<Experience> Experiences { get; set; } = new HashSet<Experience>();
  public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
