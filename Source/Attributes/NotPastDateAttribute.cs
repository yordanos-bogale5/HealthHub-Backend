using System.ComponentModel.DataAnnotations;

public class NotPastDateAttribute : ValidationAttribute {
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
    if (value == null) {
      return new ValidationResult("Date field must be provided");
    }

    if (value is not DateTime dateTime) {
      return new ValidationResult("Date should be in the correct format!");
    }

    // Compare the DateTime to the current UTC time
    if (dateTime < DateTime.UtcNow) {
      return new ValidationResult("Date cannot be set to a past time.");
    }

    return ValidationResult.Success;
  }
}
