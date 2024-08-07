using HealthHub.Source.Enums;

public class User
{
  public required int UserId { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
  public required string Password { get; set; }
  public int? Otp { get; set; }
  public string? Phone { get; set; }
  public Gender Gender { get; set; }
  public DateTime DateOfBirth { get; set; }
  public string? ProfilePicture { get; set; }
  public string? Address { get; set; }
  public Role Role { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
