using FluentValidation;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Validation.UserValidation;

/// <summary>
/// Register User Data Transfer Object Validator
/// Should be used to supplement data attribute validation in the RegisterUserDto
/// </summary>
public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
  public RegisterUserDtoValidator()
  {
    RuleFor(u => u.DateOfBirth)
      .NotEmpty()
      .WithMessage("Date of birth is required.")
      .Must(BeAValidDateTimeString)
      .WithMessage("DateOfBirth must be a valid DateTime (yyyy-MM-dd)")
      .Must(BeAtLeast18YearsOldFromString)
      .WithMessage("User must be at least 18 years old.");

    RuleFor(u => u.Role)
      .NotEmpty()
      .WithMessage("Role field is required")
      .Must(BeValidRole)
      .WithMessage("Role must be either Patient, Doctor, or Admin");

    RuleFor(u => u.Gender)
      .NotEmpty()
      .WithMessage("Gender field is required")
      .Must(BeValidGender)
      .WithMessage("Gender must be either Male or Female");

    When(
      u =>
        Enum.TryParse<Role>(u.Role?.ToString(), true, out var parsedRole)
        && parsedRole == Role.Patient,
      () =>
      {
        RuleFor(u => u.EmergencyContactName)
          .NotEmpty()
          .WithMessage("Emergency contact name is required for patients.");

        RuleFor(u => u.EmergencyContactPhone)
          .NotEmpty()
          .WithMessage("Emergency contact phone is required for patients.");
      }
    );

    When(
      u =>
        Enum.TryParse<Role>(u.Role?.ToString(), true, out var parsedRole)
        && parsedRole == Role.Doctor,
      () =>
      {
        RuleFor(u => u.Specialities)
          .NotEmpty()
          .WithMessage("At least one speciality is required for doctors.");

        RuleFor(u => u.Qualifications)
          .NotEmpty()
          .WithMessage("Qualifications are required for doctors.");

        RuleFor(u => u.Biography).NotEmpty().WithMessage("Biography is required for doctors.");

        RuleFor(u => u.DoctorStatus)
          .IsInEnum()
          .WithMessage("Doctor status must be a valid enum value.");
      }
    );
  }

  private bool BeAValidDateTimeString(string dateString)
  {
    return DateTime.TryParse(dateString, out _);
  }

  private bool BeAtLeast18YearsOldFromString(string dateString)
  {
    if (!DateTime.TryParse(dateString, out var date))
      return false;

    var today = DateTime.Today;
    var age = today.Year - date.Year;

    if (date > today.AddYears(-age))
      age--;

    return age >= 18;
  }

  private bool BeValidRole(string roleString)
  {
    return Enum.TryParse<Role>(roleString, true, out _);
  }

  public bool BeValidGender(string genderString)
  {
    return Enum.TryParse<Gender>(genderString, true, out _);
  }
}
