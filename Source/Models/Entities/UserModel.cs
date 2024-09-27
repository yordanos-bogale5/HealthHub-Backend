using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class User : BaseEntity
{
  public Guid UserId { get; set; } = Guid.NewGuid();
  public string? Auth0Id { get; set; }
  public string? Auth0AccessToken { get; set; }
  public string? Auth0RefreshToken { get; set; }

  [Required]
  public required string FirstName { get; set; }

  [Required]
  public required string LastName { get; set; }

  [Required]
  [EmailAddress]
  public required string Email { get; set; }

  public bool IsEmailVerified { get; set; } = false;
  public int? Otp { get; set; }

  [Phone]
  [MinLength(4, ErrorMessage = "The field must be at least 4 characters long.")]
  public required string Phone { get; set; }

  [Required]
  [Gender]
  public Gender Gender { get; set; }

  [Required]
  [AgeAbove(18)]
  public required DateOnly DateOfBirth { get; set; }

  public string? ProfilePicture { get; set; }

  [Required]
  public required string Address { get; set; }

  [Required]
  [RoleValidation]
  public Role Role { get; set; }

  public virtual ICollection<ConversationMembership> ConversationMemberships { get; set; } =
    new HashSet<ConversationMembership>();
  public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
  public virtual ICollection<BlogComment> BlogComments { get; set; } = new HashSet<BlogComment>();
  public virtual ICollection<BlogLike> BlogLikes { get; set; } = new HashSet<BlogLike>();
}
