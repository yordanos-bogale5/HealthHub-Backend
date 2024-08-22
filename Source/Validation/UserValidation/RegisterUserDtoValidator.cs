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
      .Must(ValidationHelper.BeAValidDateTimeString)
      .WithMessage("DateOfBirth must be a valid DateTime (yyyy-MM-dd)")
      .Must(ValidationHelper.BeAtLeast18YearsOldFromString)
      .WithMessage("User must be at least 18 years old.");

    RuleFor(u => u.Role)
      .NotEmpty()
      .WithMessage("Role field is required")
      .Must(ValidationHelper.BeValidRole)
      .WithMessage("Role must be either Patient, Doctor, or Admin");

    RuleFor(u => u.Gender)
      .NotEmpty()
      .WithMessage("Gender field is required")
      .Must(ValidationHelper.BeValidGender)
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

    // Availabilities Validation
    When(
      u => u.Availabilities.Count > 0,
      () =>
      {
        RuleFor(u => u.Availabilities)
          .Must(availabilities => availabilities.Count > 0)
          .WithMessage("At least one availability is required for doctors.");

        RuleForEach(u => u.Availabilities)
          .Must(avail => Enum.TryParse<Days>(avail.Day.ToString(), true, out _))
          .WithMessage(
            "Day must be valid! (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)"
          )
          .Must(avail => TimeOnly.TryParse(avail.StartTime.ToString(), out _))
          .WithMessage("Start time must be valid! (HH:mm)")
          .Must(avail => TimeOnly.TryParse(avail.EndTime.ToString(), out _))
          .WithMessage("End time must be valid! (HH:mm)");
      }
    );
  }
}
