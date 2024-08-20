using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes;

public class StarRatingAttribute : ValidationAttribute {
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
    if (value is int || value is float || value is double || value is decimal) {
      if (!(0 <= (decimal)value && (decimal)value <= 5)) {
        return new ValidationResult("Star rating must be between 0 - 5 inclusive");
      }
    } else {
      return new ValidationResult("Star rating must be a number!");

    }

    return ValidationResult.Success;
  }
}
