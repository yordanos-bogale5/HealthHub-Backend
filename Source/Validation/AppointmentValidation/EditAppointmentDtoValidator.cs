using FluentValidation;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Validation.AppointmentValidation;

public class EditAppointmentDtoValidator : AbstractValidator<EditAppointmentDto>
{
  public EditAppointmentDtoValidator()
  {
    When(
      ea =>
        ea.DoctorId == null
        && ea.AppointmentDate == null
        && ea.AppointmentTime == null
        && ea.AppointmentType == null,
      () =>
      {
        RuleFor(ea => ea)
          .NotEmpty()
          .WithMessage("At least one field is required to update an appointment.");
      }
    );

    When(
      ea => ea.DoctorId != null,
      () =>
      {
        RuleFor(ea => ea.DoctorId)
          .NotEmpty()
          .WithMessage("DoctorId is required.")
          .Must(ValidationHelper.BeAValidGuid)
          .WithMessage("DoctorId must be a valid GUID.");
      }
    );

    When(
      ea => ea.AppointmentDate != null,
      () =>
      {
        RuleFor(ea => ea.AppointmentDate)
          .NotEmpty()
          .WithMessage("AppointmentDate is required.")
          .Must(ValidationHelper.BeAValidDateTimeString)
          .WithMessage("AppointmentDate must be a valid DateTime (yyyy-MM-dd)")
          .Must(ValidationHelper.BeNotPastDate)
          .WithMessage("AppointmentDate must not be in the past.");
      }
    );

    When(
      ea => ea.AppointmentTime != null,
      () =>
      {
        RuleFor(ea => ea.AppointmentTime)
          .NotEmpty()
          .WithMessage("AppointmentTime is required.")
          .Must(ValidationHelper.BeAValidDateTimeString)
          .WithMessage("AppointmentTime must be a valid DateTime (HH:mm)")
          .Must(ValidationHelper.BeNotPastDate)
          .WithMessage("AppointmentDate must not be in the past.");
      }
    );

    When(
      ea => ea.AppointmentType != null,
      () =>
      {
        RuleFor(ea => ea.AppointmentType)
          .NotEmpty()
          .WithMessage("Appointment Type is required.")
          .Must(ValidationHelper.BeAValidAppointmentType)
          .WithMessage(
            $"Appointment can only be {string.Join(", ", Enum.GetNames(typeof(AppointmentType)))}"
          );
      }
    );
  }
}
