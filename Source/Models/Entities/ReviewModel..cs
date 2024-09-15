using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;

namespace HealthHub.Source.Models.Entities;

public class Review : BaseEntity
{
  public Guid ReviewId { get; set; }

  [Required]
  public Guid PaymentId { get; set; } // <<FK>>

  [Required]
  [StarRating]
  public decimal StarRating { get; set; }

  [Required]
  public required string ReviewText { get; set; }

  public virtual required Payment Payment { get; set; }
}
