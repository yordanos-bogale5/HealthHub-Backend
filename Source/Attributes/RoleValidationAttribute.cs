using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes;

public class RoleValidationAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
    {
      return new ValidationResult("Role is required.");
    }

    if (value.ToString() != "Admin" && value.ToString() != "Doctor" && value.ToString() != "Patient")
    {
      return new ValidationResult("Role should be either Admin, Doctor, or Patient.");
    }

    return ValidationResult.Success;
  }
}