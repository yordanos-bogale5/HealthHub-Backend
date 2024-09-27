using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class BlogComment : BaseEntity
{
  public Guid BlogCommentId { get; set; }

  public Guid BlogId { get; set; } // <<FK>>

  public Guid SenderId { get; set; } // <<FK>>

  public required string CommentText { get; set; }

  public virtual Blog? Blog { get; set; } // <<NAV>>
  public virtual User? Sender { get; set; } // <<NAV>>
}
