using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Blog : BaseEntity
{
  public Guid BlogId { get; set; }

  [Required]
  public Guid AuthorId { get; set; } // <<FK>>

  [Required]
  public required string Title { get; set; }

  [Required]
  public required string Content { get; set; }

  public virtual required User Author { get; set; }

  public virtual ICollection<BlogComment> BlogComments { get; set; } = new HashSet<BlogComment>();

  public virtual ICollection<BlogLike> BlogLikes { get; set; } = new HashSet<BlogLike>();
}
