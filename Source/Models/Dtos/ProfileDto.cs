using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public interface IProfileDto
{
  Guid UserId { get; set; }
  string FirstName { get; set; }
  string LastName { get; set; }
  string Email { get; set; }
  string ProfilePicture { get; set; }
}

public class ProfileDtoBase : IProfileDto
{
  public required Guid UserId { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
  public required string ProfilePicture { get; set; }
}

public class BlogProfileDto : ProfileDtoBase;

public class ConversationProfileDto : ProfileDtoBase;

public record ProfileDto
{
  public required Guid UserId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
  public required string ProfilePicture { get; init; }
  public required string Phone { get; init; }
  public required Gender Gender { get; init; }
  public required DateOnly DateOfBirth { get; init; }
  public required string Address { get; init; }
  public required Role Role { get; init; }
}

public record Auth0ProfileDto
{
  public Guid UserId { get; init; } = Guid.Empty;
  public string FirstName { get; init; } = string.Empty;
  public string LastName { get; init; } = string.Empty;
  public Role Role { get; init; }
  public string Phone { get; init; } = string.Empty;
  public Gender Gender { get; init; }
  public string DateOfBirth { get; init; } = string.Empty;
}

/// <summary>
/// The patient user profile we return to the client when requested for
/// </summary>
public record PatientProfileDto : ProfileDto
{
  public required Guid PatientId { get; init; }
  public required string MedicalHistory { get; init; }
  public required string EmergencyContactName { get; init; }
  public required string EmergencyContactPhone { get; init; }
};

/// <summary>
/// The doctor user profile we return to the client when requested for
/// </summary>
public record DoctorProfileDto : ProfileDto
{
  public required Guid DoctorId { get; init; }
  public required List<string> Specialities { get; init; }
  public required List<AvailabilityDto> Availabilities { get; init; }
  public required string Qualifications { get; init; }
  public required string Biography { get; init; }
  public required DoctorStatus DoctorStatus { get; init; }
  public required List<EducationDto> Educations { get; init; }
  public required List<ExperienceDto> Experiences { get; init; }
};

/// <summary>
/// The payload we expect the client to provide to edit the profile information of a user
/// </summary>
public record EditProfileDto
{
  public required string UserId { get; init; }
  public string? FirstName { get; init; }
  public string? LastName { get; init; }

  [EmailAddress]
  public string? Email { get; init; }
  public string? ProfilePicture { get; init; }

  [Phone]
  public string? Phone { get; init; }

  public string? Gender { get; init; }
  public string? DateOfBirth { get; init; }
  public string? Address { get; init; }

  // If the user is a patient, they may/may-not specify the following
  // Note the following fields MUST be validated in the controller based on the Role field provided as payload
  public string? MedicalHistory { get; set; }
  public string? EmergencyContactName { get; set; }
  public string? EmergencyContactPhone { get; set; }

  // If the user is a doctor, they may/may-not specify the following
  // Note the following fields MUST be validated in the controller based on the Role field provided as payload
  public List<string>? Specialities { get; set; }
  public List<AvailabilityDto>? Availabilities { get; set; }
  public string? Qualifications { get; set; }
  public string? Biography { get; set; }
  public string? DoctorStatus { get; set; }
  public CreateFileDto? Cv { get; set; }
  public List<EditEducationDto>? Educations { get; set; }
  public List<EditExperienceDto>? Experiences { get; set; }
}
