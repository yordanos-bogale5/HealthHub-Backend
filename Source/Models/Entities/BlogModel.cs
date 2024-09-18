using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Blog : BaseEntity
{
  public Guid BlogId { get; set; } = Guid.NewGuid();

  public required Guid AuthorId { get; set; } // <<FK>>

  public required string Title { get; set; }

  public required string Content { get; set; }

  public required string Slug { get; set; }

  public required string Summary { get; set; }

  public virtual User? Author { get; set; }

  public virtual ICollection<BlogComment> BlogComments { get; set; } = new HashSet<BlogComment>();

  public virtual ICollection<BlogLike> BlogLikes { get; set; } = new HashSet<BlogLike>();

  public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}
