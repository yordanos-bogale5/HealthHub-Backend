using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes;

public class PasswordAttribute : ValidationAttribute {
  public int MinimumLength { get; set; } = 8; // Default minimum length

  public PasswordAttribute() {
  }

  public PasswordAttribute(int minimumLength) {
    MinimumLength = minimumLength;
  }

  protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
    if (value == null) {
      return new ValidationResult("Password is required.");
    }

    string password = value.ToString() ?? "";

    if (password.Length < MinimumLength) {
      return new ValidationResult($"Password must be at least {MinimumLength} characters long.");
    }

    if (!password.Any(char.IsDigit)) {
      return new ValidationResult("Password must contain at least one digit.");
    }

    if (!password.Any(char.IsUpper)) {
      return new ValidationResult("Password must contain at least one uppercase letter.");
    }

    if (!password.Any(char.IsLower)) {
      return new ValidationResult("Password must contain at least one lowercase letter.");
    }

    return ValidationResult.Success!;
  }
}
