using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Attributes;

public class RoleValidationAttribute : ValidationAttribute {
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
    if (value == null) {
      return new ValidationResult("Role is required.");
    }

    if (value is not Role role) {
      return new ValidationResult("Invalid role type.");
    }

    // Validate if the role is one of the allowed values
    if (!Enum.IsDefined(typeof(Role), role)) {
      return new ValidationResult("Role should be either Admin, Doctor, or Patient.");
    }

    return ValidationResult.Success;
  }
}
