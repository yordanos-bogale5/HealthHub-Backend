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


public record ProfileDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePicture,
    string Phone,
    Gender Gender,
    DateTime DateOfBirth,
    string Address,
    Role Role
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
/// These are the fields what a newly created Auth0 User will have
/// </summary>
/// <param name="UserId"></param>
/// <param name="Profile"></param>
/// <param name="EmailVerified"></param>
public record Auth0UserDto(
  string UserId,
  string Profile,
  bool EmailVerified
);


/// <summary>
/// Auth0 Login Data Transfer Object. Used for returning Auth0 login information to the client.
/// </summary>
/// <param name="AccessToken"></param>
/// <param name="ExpiresIn"></param>
public record Auth0LoginDto(
  string AccessToken,
  int ExpiresIn
);


/// <summary>
/// Login User Data Transfer Object. Sent from the client to the server for logging in a user. Used to validate client request upon hitting login endpoint.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginUserDto(
  [Required][EmailAddress] string Email,
  [Required] string Password
);