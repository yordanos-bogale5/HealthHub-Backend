using FluentValidation;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Validation.AppointmentValidation;

public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
{
  public CreateAppointmentDtoValidator()
  {
    RuleFor(ca => ca.DoctorId)
      .NotEmpty()
      .WithMessage("DoctorId is required.")
      .Must(ValidationHelper.BeAValidGuid)
      .WithMessage("DoctorId must be a valid GUID.");

    RuleFor(ca => ca.PatientId)
      .NotEmpty()
      .WithMessage("PatientId is required.")
      .Must(ValidationHelper.BeAValidGuid)
      .WithMessage("PatientId must be a valid GUID.");

    RuleFor(ca => ca.AppointmentDate)
      .NotEmpty()
      .WithMessage("AppointmentDate is required.")
      .Must(ValidationHelper.BeAValidDateTimeString)
      .WithMessage("AppointmentDate must be a valid DateTime (yyyy-MM-dd)")
      .Must(ValidationHelper.BeNotPastDate)
      .WithMessage("AppointmentDate must not be in the past.");

    RuleFor(ca => ca.AppointmentTime)
      .NotEmpty()
      .WithMessage("AppointmentTime is required.")
      .Must(ValidationHelper.BeAValidDateTimeString)
      .WithMessage("AppointmentTime must be a valid DateTime (HH:mm)")
      .Must(ValidationHelper.BeNotPastDate)
      .WithMessage("AppointmentDate must not be in the past.");

    RuleFor(ca => ca.AppointmentType)
      .NotEmpty()
      .WithMessage("Appointment Type is required.")
      .Must(ValidationHelper.BeAValidAppointmentType)
      .WithMessage(
        $"Appointment can only be {string.Join(", ", Enum.GetNames(typeof(AppointmentType)))}"
      );
  }
}
