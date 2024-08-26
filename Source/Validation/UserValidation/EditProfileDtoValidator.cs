using FluentValidation;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Validation;

public class EditProfileDtoValidator : AbstractValidator<EditProfileDto>
{
  public EditProfileDtoValidator()
  {
    When(
      u =>
        u.FirstName == null
        && u.LastName == null
        && u.Email == null
        && u.ProfilePicture == null
        && u.Phone == null
        && u.Gender == null
        && u.DateOfBirth == null
        && u.Address == null
        && u.Role == null
        && u.MedicalHistory == null
        && u.EmergencyContactName == null
        && u.EmergencyContactPhone == null
        && u.Specialities == null
        && u.Availabilities == null
        && u.Qualifications == null
        && u.Biography == null
        && u.DoctorStatus == null,
      () =>
      {
        RuleFor(u => u)
          .NotEmpty()
          .WithMessage("At least one field is required to update a user profile.");
      }
    );

    When(
      u => u.FirstName != null,
      () =>
      {
        RuleFor(u => u.FirstName).NotEmpty().WithMessage("First name is required.");
      }
    );

    When(
      u => u.LastName != null,
      () =>
      {
        RuleFor(u => u.LastName).NotEmpty().WithMessage("Last name is required.");
      }
    );

    When(
      u => u.DateOfBirth != null,
      () =>
      {
        RuleFor(u => u.DateOfBirth)
          .NotEmpty()
          .WithMessage("Date of birth is required.")
          .Must(ValidationHelper.BeAValidDateTimeString)
          .WithMessage("DateOfBirth must be a valid DateTime (yyyy-MM-dd)")
          .Must(ValidationHelper.BeAtLeast18YearsOldFromString)
          .WithMessage("User must be at least 18 years old.");
      }
    );

    When(
      u => u.Role != null,
      () =>
      {
        RuleFor(u => u.Role)
          .NotEmpty()
          .WithMessage("Role field is required")
          .Must(ValidationHelper.BeValidRole)
          .WithMessage("Role must be either Patient, Doctor, or Admin");
      }
    );

    When(
      u => u.Gender != null,
      () =>
      {
        RuleFor(u => u.Gender)
          .NotEmpty()
          .WithMessage("Gender field is required")
          .Must(ValidationHelper.BeValidGender)
          .WithMessage("Gender must be either Male or Female");
      }
    );

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
      u => u.Availabilities != null && u.Availabilities.Count > 0,
      () =>
      {
        RuleFor(u => u.Availabilities)
          .Must(availabilities => availabilities != null && availabilities.Count > 0)
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
