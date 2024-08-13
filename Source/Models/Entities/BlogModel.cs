using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Blog
{
  public Guid BlogId { get; set; }
  [Required]
  public Guid AuthorId { get; set; }  // <<FK>>
  [Required]
  public required string Title { get; set; }
  [Required]
  public required string Content { get; set; }

  public virtual User? User { get; set; }

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}