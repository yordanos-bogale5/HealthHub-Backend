using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Dtos;

/// <summary>
/// User Data Transfer Object. Used for returning user information to the client.
/// </summary>
/// <param name="UserId"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Email"></param>
/// <param name="ProfilePicture"></param>
/// <param name="Gender"></param>
public record UserDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePicture,
    Gender Gender
);


/// <summary>
/// Register User Data Transfer Object. Sent from the client to the server for registering a new user. Used to validate client request upon hitting register endpoint.
/// </summary>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="Phone"></param>
/// <param name="Gender"></param>
/// <param name="DateOfBirth"></param>
/// <param name="ProfilePicture"></param>
/// <param name="Address"></param>
public record RegisterUserDto(
  [Required] string FirstName,
  [Required] string LastName,
  [Required][EmailAddress] string Email,
  [Required][Password][PasswordPropertyText] string Password,
  [Required][Phone][MinLength(4, ErrorMessage = "The field must be at least 4 characters long.")] string Phone,
  [Required] Gender Gender,
  [Required][AgeAbove18] DateTime DateOfBirth,
  string? ProfilePicture,
  [Required] string Address
);



/// <summary>
/// Login User Data Transfer Object. Sent from the client to the server for logging in a user. Used to validate client request upon hitting login endpoint.
/// </summary>
/// <param name="email"></param>
/// <param name="password"></param>
public record LoginUserDto(
  [Required][EmailAddress] string email,
  [Required] string password
);