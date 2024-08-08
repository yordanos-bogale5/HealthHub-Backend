using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Enums;

public class User
{
  public int UserId { get; set; }
  [Required]
  public required string FirstName { get; set; }
  [Required]
  public required string LastName { get; set; }
  [Required]
  [EmailAddress]
  public required string Email { get; set; }
  [Required]
  [Password]
  [PasswordPropertyText]
  public required string Password { get; set; }
  public int? Otp { get; set; }
  [Phone]
  [MinLength(4, ErrorMessage = "The field must be at least 4 characters long.")]
  public required string Phone { get; set; }
  [Required]
  public Gender Gender { get; set; }
  [Required]
  [AgeAbove18]
  public DateTime DateOfBirth { get; set; }
  public string? ProfilePicture { get; set; }
  [Required]
  public required string Address { get; set; }
  [Required]
  public Role Role { get; set; }
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
