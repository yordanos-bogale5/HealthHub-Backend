using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Dtos;

// Request DTO for creating a blog
public record CreateBlogDto
{
  [Required]
  [Guid]
  public required Guid AuthorId { get; set; }

  [Required]
  [MinLength(5)]
  [MaxLength(100)]
  public required string Title { get; set; }

  [Required]
  [MinLength(100)]
  [MaxLength(10_000)]
  public required string Content { get; set; }

  [Required]
  [MinLength(1)]
  public required string Slug { get; set; }

  [Required]
  [MinLength(10)]
  [MaxLength(255)]
  public required string Summary { get; set; }

  public IList<string> Tags { get; set; } = [];
};

public record EditBlogDto
{
  [Required]
  [MinLength(5)]
  [MaxLength(100)]
  public required string Title { get; set; }

  [Required]
  [MinLength(100)]
  [MaxLength(10_000)]
  public required string Content { get; set; }

  [Required]
  [MinLength(1)]
  public required string Slug { get; set; }

  [Required]
  [MinLength(10)]
  [MaxLength(255)]
  public required string Summary { get; set; }

  public IList<string> Tags { get; set; } = [];
};

public record BlogDto
{
  public required Guid BlogId { get; set; }
  public required Guid AuthorId { get; set; }
  public required string Title { get; set; }
  public required string Content { get; set; }
  public required string Slug { get; set; }
  public IList<string> Tags { get; set; } = [];
  public required string Summary { get; set; }
  public required BlogProfileDto Author { get; set; }
  public required ICollection<BlogLikeDto> BlogLikes { get; set; } = new HashSet<BlogLikeDto>();
};

// Request DTO for creating a blog comment
public record CreateBlogCommentDto
{
  [Required]
  [Guid]
  public required Guid BlogId { get; set; }

  [Required]
  [Guid]
  public required Guid SenderId { get; set; }

  [Required]
  [MinLength(1)]
  public required string CommentText { get; set; }
}

public record BlogCommentDto
{
  public required Guid BlogCommentId { get; set; }
  public required Guid BlogId { get; set; }
  public required Guid SenderId { get; set; }
  public required string CommentText { get; set; }
  public required BlogProfileDto Sender { get; set; }
}

public record BlogProfileDto(
  Guid UserId,
  string FirstName,
  string LastName,
  string Email,
  string ProfilePicture
);

// Request DTO for creating a blog like
public record CreateBlogLikeDto([Required] [Guid] Guid UserId, [Required] [Guid] Guid BlogId);

public record BlogLikeDto(Guid BlogLikeId, Guid UserId, Guid BlogId, BlogProfileDto User);
