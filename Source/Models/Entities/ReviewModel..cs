using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;

namespace HealthHub.Source.Models.Entities;

public class Review : BaseEntity
{
  public Guid ReviewId { get; set; }

  [Required]
  public Guid DoctorId { get; set; } // <<FK>>

  [Required]
  public Guid PatientId { get; set; } // <<FK>>

  [Required]
  [StarRating]
  [Range(0, 5)]
  public decimal StarRating { get; set; }

  [Required]
  public required string ReviewText { get; set; }

  public virtual Doctor? Doctor { get; set; }
  public virtual Patient? Patient { get; set; }
}
