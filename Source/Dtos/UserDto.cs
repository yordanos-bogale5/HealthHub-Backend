using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Dtos;

public record RegisterUserDto(
  [Required] string FirstName,
  [Required] string LastName,
  [Required] string Email,
  [Required] string Password,
  [Required][Phone] string Phone,
  [Required] Gender Gender,
  [Required][AgeAbove18] DateTime DateOfBirth,
  string? ProfilePicture,
  [Required] string Address
);