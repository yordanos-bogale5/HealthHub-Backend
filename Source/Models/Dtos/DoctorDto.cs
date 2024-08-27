using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public record CreateDoctorDto
{
  public required User User { get; init; }
  public required string Qualifications { get; init; }
  public required string Biography { get; init; }
  public DoctorStatus DoctorStatus { get; init; } = DoctorStatus.Active;
}

public record EditDoctorProfileDto(
  Guid UserId,
  List<string>? Specialitites,
  string? Qualifications,
  string? Biography,
  List<AvailabilityDto>? Availabilities,
  DoctorStatus? DoctorStatus
);

public record DoctorDto
{
  public required Guid UserId { get; init; }
  public required Guid DoctorId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
  public required bool IsEmailVerified { get; set; } = false;
  public required string Phone { get; init; }
  public required Gender Gender { get; init; }
  public required DateTime DateOfBirth { get; init; }
  public required string ProfilePicture { get; set; }
  public required string Address { get; init; }

  public required List<string> Specialities { get; set; }
  public required string Qualifications { get; set; }
  public required string Biography { get; set; }
  public required DoctorStatus DoctorStatus { get; set; }
}
