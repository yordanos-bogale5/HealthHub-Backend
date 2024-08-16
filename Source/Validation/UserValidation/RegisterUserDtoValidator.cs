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
    // What is remaining from the RegisterUserDto validation using Attribues is
    // Role based Validation

    // Validate fields required for Patients
    When(u => u.Role == Role.Patient, () =>
    {
      RuleFor(u => u.MedicalHistory)
                  .NotEmpty().WithMessage("Medical history is required for patients.");

      RuleFor(u => u.EmergencyContactName)
                  .NotEmpty().WithMessage("Emergency contact name is required for patients.");

      RuleFor(u => u.EmergencyContactPhone)
                  .NotEmpty().WithMessage("Emergency contact phone is required for patients.");
    });

    // Validate fields required for Doctors
    When(u => u.Role == Role.Doctor, () =>
    {
      RuleFor(u => u.Specialities)
                  .NotEmpty().WithMessage("At least one speciality is required for doctors.");

      RuleFor(u => u.Qualifications)
                  .NotEmpty().WithMessage("Qualifications are required for doctors.");

      RuleFor(u => u.Biography)
                  .NotEmpty().WithMessage("Biography is required for doctors.");

      RuleFor(u => u.DoctorStatus)
                  .IsInEnum().WithMessage("Doctor status must be a valid enum value.");
    });

  }
}