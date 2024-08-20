using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Admin {
  public Guid AdminId { get; set; }

  [Required]
  public required Guid UserId { get; set; } // <<FK>>

  public virtual required User User { get; set; }
}
