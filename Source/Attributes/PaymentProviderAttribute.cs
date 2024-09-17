using System;
using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

public class PaymentProviderAttribute : ValidationAttribute
{
  private readonly bool _required;

  public PaymentProviderAttribute(bool required = true)
  {
    _required = required;
  }

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    // If the value is null and the field is required, return an error
    if (value is null && _required)
    {
      return new ValidationResult("Payment provider is required.");
    }

    // Check if the value is a valid PaymentProvider enum
    if (value is PaymentProvider paymentProvider)
    {
      if (
        !Enum.IsDefined(typeof(PaymentProvider), paymentProvider)
        || paymentProvider == PaymentProvider.Unknown
      )
      {
        var providers = Enum.GetNames<PaymentProvider>();
        return new ValidationResult(
          $"Invalid payment provider. Payment providers can only be {string.Join(", ", providers.Skip(1))}."
        );
      }
    }
    else
    {
      return new ValidationResult("Invalid payment provider value.");
    }
    return ValidationResult.Success;
  }
}
