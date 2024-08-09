using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Dtos;

public record UserDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePicture,
    Gender Gender
);


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


