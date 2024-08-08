using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Dtos;

public record RegisterUserDto(
  [Required] string FirstName,
  [Required] string LastName,
  [Required] string Email,
  [Required] string Password,
  [Required][Phone] string Phone,
  [Required] Gender Gender,
  [Required] DateTime DateOfBirth,
  string? ProfilePicture,
  [Required] string Address
);