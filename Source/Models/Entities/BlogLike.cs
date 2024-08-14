using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class BlogLike
{
  public Guid BlogLikeId { get; set; } = Guid.NewGuid();
  [Required]
  public Guid UserId { get; set; } // <<FK>>
  [Required]
  public Guid BlogId { get; set; } // <<FK>>

  public virtual User? User { get; set; }
  public virtual Blog? Blog { get; set; }
}