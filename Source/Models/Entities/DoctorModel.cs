using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Entities;

public class Doctor
{
  public Guid DoctorId { get; set; }
  [Required]
  public Guid UserId { get; set; }  // <<FK>>
  [Required]
  public Guid SpecialityId { get; set; }  // <<FK>>

  [Required]
  public required string Qualifications { get; set; }
  [Required]
  public required string Biography { get; set; }
  public DoctorStatus DoctorStatus { get; set; } = DoctorStatus.Active;

  public virtual User? User { get; set; } // <<NAV>>
  public virtual Speciality? Speciality { get; set; } // <<NAV>>
}