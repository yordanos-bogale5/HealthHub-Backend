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
      .Must(BeAValidGuid)
      .WithMessage("DoctorId must be a valid GUID.");

    RuleFor(ca => ca.PatientId)
      .NotEmpty()
      .WithMessage("PatientId is required.")
      .Must(BeAValidGuid)
      .WithMessage("PatientId must be a valid GUID.");

    RuleFor(ca => ca.AppointmentDate)
      .NotEmpty()
      .WithMessage("AppointmentDate is required.")
      .Must(BeAValidDateTimeString)
      .WithMessage("AppointmentDate must be a valid DateTime (yyyy-MM-dd)")
      .Must(BeNotPastDate)
      .WithMessage("AppointmentDate must not be in the past.");

    RuleFor(ca => ca.AppointmentTime)
      .NotEmpty()
      .WithMessage("AppointmentTime is required.")
      .Must(BeAValidDateTimeString)
      .WithMessage("AppointmentTime must be a valid DateTime (HH:mm)")
      .Must(BeNotPastDate)
      .WithMessage("AppointmentDate must not be in the past.");

    RuleFor(ca => ca.AppointmentType)
      .NotEmpty()
      .WithMessage("Appointment Type is required.")
      .Must(BeAValidAppointmentType)
      .WithMessage(
        $"Appointment can only be {string.Join(", ", Enum.GetNames(typeof(AppointmentType)))}"
      );
  }

  public bool BeAValidGuid(string id)
  {
    return Guid.TryParse(id, out _);
  }

  public bool BeAValidDateTimeString(string date)
  {
    return DateTime.TryParse(date, out _);
  }

  public bool BeAValidAppointmentType(string appointmentTypeString)
  {
    return Enum.TryParse<AppointmentType>(appointmentTypeString, out _);
  }

  public bool BeNotPastDate(string date)
  {
    return DateTime.TryParse(date, out var parsedDate) && parsedDate.Date >= DateTime.UtcNow.Date;
  }
}
