using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Dtos;

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
