using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class BlogComment
{
    public Guid BlogCommentId { get; set; }

    [Required]
    public Guid BlogId { get; set; } // <<FK>>

    [Required]
    public Guid SenderId { get; set; } // <<FK>>

    [Required]
    public required string CommentText { get; set; }

    public virtual required Blog Blog { get; set; } // <<NAV>>
    public virtual required User Sender { get; set; } // <<NAV>>
}
