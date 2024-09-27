using FluentValidation;
using HealthHub.Source.Helpers;
using HealthHub.Source.Models.Defaults;
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
          .NotNull()
          .WithMessage("Specialities cannot be null")
          .NotEmpty()
          .WithMessage("At least one speciality is required for doctors.");

        RuleFor(u => u.Qualifications)
          .NotNull()
          .WithMessage("Qualifications cannot be null")
          .NotEmpty()
          .WithMessage("Qualifications are required for doctors.");

        RuleFor(u => u.Biography).NotEmpty().WithMessage("Biography is required for doctors.");

        RuleFor(u => u.DoctorStatus)
          .IsInEnum()
          .WithMessage("Doctor status must be a valid enum value.");

        RuleFor(u => u.Cv)
          .NotNull()
          .WithMessage("Cv cannot be null")
          .NotEmpty()
          .WithMessage("Cv is required for doctors!")
          .Must(cv => cv != null && ValidationHelper.IsImageMime(cv.MimeType))
          .WithMessage(
            $"The mime type provided is not valid. Available mime-types are {string.Join(",", Mime.GetImageMimes().Select(m => Mime.GetMime(m)))}"
          )
          .Must(cv => cv != null && ValidationHelper.IsValidBase64(cv.FileDataBase64))
          .WithMessage("The file data is not a valid base64.");

        RuleFor(u => u.Availabilities)
          .NotEmpty()
          .WithMessage("Availability cannot be empty.")
          .Must(availabilities => availabilities.Count > 0)
          .WithMessage("At least one availability is required for doctors.");

        RuleForEach(u => u.Availabilities)
          .NotEmpty()
          .WithMessage("Availability Cannot be empty")
          .Must(avail => Enum.TryParse<DayOfWeek>(avail.Day.ToString(), true, out _))
          .WithMessage(
            "Day must be valid! (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)"
          )
          .Must(avail => TimeOnly.TryParse(avail.StartTime.ToString(), out _))
          .WithMessage("Start time must be valid! (HH:mm)")
          .Must(avail => TimeOnly.TryParse(avail.EndTime.ToString(), out _))
          .WithMessage("End time must be valid! (HH:mm)");

        RuleFor(u => u.OnlineAppointmentFee)
          .NotEmpty()
          .WithMessage("Online Appointment fee cannot be empty.")
          // Change the below based on the business requirement
          .Must(appFee => appFee >= 0)
          .WithMessage("Online Appointment fee cannot be negative!");

        RuleFor(u => u.InPersonAppointmentFee)
          .NotEmpty()
          .WithMessage("Inperson Appointment fee cannot be empty.")
          // Change the below based on the business requirement
          .Must(appFee => appFee >= 0)
          .WithMessage("Inperson Appointment fee cannot be negative!");

        RuleFor(u => u.Educations)
          .NotNull()
          .NotEmpty()
          .WithMessage("Educations cannot be empty.")
          .Must(educations => educations.Count > 0)
          .WithMessage("At least one education is required for doctors.");

        RuleForEach(u => u.Educations)
          .NotEmpty()
          .WithMessage("Education is required.")
          .Must(education => education.Degree.Trim().Length > 0)
          .WithMessage("Education degree must be provided.")
          .Must(education => education.Institution.Trim().Length > 0)
          .WithMessage("Education Institution must be provided.")
          .Must(education => ValidationHelper.BeAValidDateOnlyString(education.StartDate))
          .WithMessage("Start date should be a valid date! (yyyy-MM-dd)")
          .Must(education => ValidationHelper.BeAValidDateOnlyString(education.EndDate))
          .WithMessage("End date should be a valid date! (yyyy-MM-dd)");

        RuleFor(u => u.Experiences)
          .NotNull()
          .NotEmpty()
          .WithMessage("Experiences cannot be empty.")
          .Must(experiences => experiences.Count > 0)
          .WithMessage("At least one experience is required for doctors.");

        RuleForEach(u => u.Experiences)
          .NotNull()
          .WithMessage("Experiences cannot be null.")
          .NotEmpty()
          .Must(experience => experience.Institution.Trim().Length > 0)
          .WithMessage("Please provide the institution of your experience.")
          .Must(experience => ValidationHelper.BeAValidDateOnlyString(experience.StartDate))
          .WithMessage("Start date should be a valid date! (yyyy-MM-dd)")
          .Must(experience =>
            experience.EndDate == null
            || ValidationHelper.BeAValidDateOnlyString(experience.EndDate)
          )
          .WithMessage(
            "End date should either be null or a valid date! (yyyy-MM-dd). If its null then the person is still working in the specified institute."
          );
      }
    );
  }
}
