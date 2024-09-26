using System;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;
using Newtonsoft.Json;

public class PaymentProviderAttribute(bool required = true) : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var validProviders = Enum.GetNames<PaymentProvider>().Skip(1); // Skips "Unknown"

    // If required and value is null or empty
    if (required && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
    {
      return new ValidationResult("Payment provider is required.");
    }

    // If value is present, validate it
    if (
      value != null
      && (
        !Enum.TryParse<PaymentProvider>(value.ToString(), out var provider)
        || provider == PaymentProvider.Unknown
      )
    )
    {
      return new ValidationResult(
        $"Invalid payment provider. Valid options: {string.Join(", ", validProviders)}"
      );
    }

    // Return success by default
    return ValidationResult.Success;
  }
}
