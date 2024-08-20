using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes;

public class AgeAboveAttribute(int minAge = 18, string errMsg = "You must be at least 18 years old.") : ValidationAttribute {
  protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
    if (value == null) {
      return new ValidationResult("Date field must be provided.");
    }

    if (value is not DateTime birthDate) {
      return new ValidationResult("Date should be in the correct format.");
    }

    var today = DateTime.Today;
    var age = today.Year - birthDate.Year;
    if (birthDate > today.AddYears(-age)) age--; // Subtract 1 if birthday hasn't occurred yet

    if (age >= minAge) {
      return ValidationResult.Success!;
    }
    return new ValidationResult(FormatErrorMessage(null));
  }

  public override string FormatErrorMessage(string? name) {
    return errMsg;
  }
}

