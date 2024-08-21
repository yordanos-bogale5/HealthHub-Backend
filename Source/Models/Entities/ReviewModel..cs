using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;

namespace HealthHub.Source.Models.Entities;

public class Review
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

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
