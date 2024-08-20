using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class BlogLike {
  public Guid BlogLikeId { get; set; } = Guid.NewGuid();

  [Required]
  public Guid UserId { get; set; } // <<FK>>

  [Required]
  public Guid BlogId { get; set; } // <<FK>>

  public virtual required User User { get; set; }
  public virtual required Blog Blog { get; set; }
}
