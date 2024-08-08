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
  public required string Password { get; set; }
  public int? Otp { get; set; }
  [Phone]
  public required string Phone { get; set; }
  [Required]
  public Gender Gender { get; set; }
  [Required]
  [AgeAbove18]
  public DateTime DateOfBirth { get; set; }
  public string? ProfilePicture { get; set; }
  public string? Address { get; set; }
  [Required]
  public Role Role { get; set; }
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
