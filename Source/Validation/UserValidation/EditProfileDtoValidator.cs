using FluentValidation;
using FluentValidation.Results;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Validation;

public class EditProfileDtoValidator : AbstractValidator<EditProfileDto>
{
  ApplicationContext appContext;

  public EditProfileDtoValidator(ApplicationContext appContext)
  {
    this.appContext = appContext;

    RuleFor(u => u)
      .Custom(
        (u, context) =>
        {
          if (
            u.FirstName == null
            && u.LastName == null
            && u.Email == null
            && u.ProfilePicture == null
            && u.Phone == null
            && u.Gender == null
            && u.DateOfBirth == null
            && u.Address == null
            && u.MedicalHistory == null
            && u.EmergencyContactName == null
            && u.EmergencyContactPhone == null
            && u.Specialities == null
            && u.Availabilities == null
            && u.Qualifications == null
            && u.Biography == null
            && u.DoctorStatus == null
          )
          {
            context.AddFailure(
              "Payload",
              @"At least one field is required to update a user profile. Valid fields are FirstName, LastName, Email, ProfilePicture, Phone, Gender, DateOfBirth, Address, MedicalHistory, EmergencyContactName, EmergencyContactPhone, Specialities, Availabilities, Qualifications, Biography and DoctorStatus"
            );
          }
        }
      );

    RuleFor(u => u.UserId)
      .NotEmpty()
      .WithMessage("User Id must be provided to edit a profile")
      .Must(uid => Guid.TryParse(uid, out _))
      .WithMessage("Not valid Guid");
  }

  public override async Task<ValidationResult> ValidateAsync(
    ValidationContext<EditProfileDto> context,
    CancellationToken cancellation = default
  )
  {
    EditProfileDto v = context.InstanceToValidate;

    var preValidationResult = await base.ValidateAsync(context, cancellation);

    if (!preValidationResult.IsValid)
    {
      return preValidationResult;
    }

    var user = appContext.Users.Where(us => us.UserId == v.UserId.ToGuid()).FirstOrDefault();
    if (user == null)
    {
      return await Task.FromResult(
        new ValidationResult(
          new List<ValidationFailure> { new(nameof(v.UserId), "User with that Id is not found.") }
        )
      );
    }

    Role role = user.Role; // User Role

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
      u => u.Gender != null,
      () =>
      {
        RuleFor(u => u.Gender)
          .NotEmpty()
          .WithMessage("Gender field is required")
          .Must(ValidationHelper.BeValidGender)
          .WithMessage($"Gender must be {string.Join(",", Enum.GetNames(typeof(Gender)))}");
      }
    );

    if (role == Role.Patient)
    {
      RuleFor(u => u.EmergencyContactName)
        .NotEmpty()
        .WithMessage("Emergency contact name is required for patients.");

      RuleFor(u => u.EmergencyContactPhone)
        .NotEmpty()
        .WithMessage("Emergency contact phone is required for patients.");
    }

    if (role == Role.Doctor)
    {
      When(
        u => u.Qualifications != null,
        () =>
        {
          RuleFor(u => u.Qualifications)
            .NotEmpty()
            .WithMessage("Qualifications are required for doctors.");
        }
      );
      When(
        u => u.Biography != null,
        () =>
        {
          RuleFor(u => u.Biography).NotEmpty().WithMessage("Biography is required for doctors.");
        }
      );
      When(
        u => u.DoctorStatus != null,
        () =>
        {
          RuleFor(u => u.DoctorStatus)
            .NotEmpty()
            .WithMessage(
              $"Doctor status cannot be empty. Choose either {string.Join(",", Enum.GetNames(typeof(DoctorStatus)))}"
            )
            .Must(ValidationHelper.BeValidDoctorStatus)
            .WithMessage("Doctor status must be a valid enum value.");
        }
      );
    }

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

    return await base.ValidateAsync(context);
  }
}
