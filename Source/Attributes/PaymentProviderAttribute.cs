using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

public class PaymentProviderAttribute(bool required = true) : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value is null && required)
      return new ValidationResult("Payment provider is required.");

    var providers = Enum.GetNames<PaymentProvider>();

    if (
      value is not PaymentProvider paymentProvider
      || !Enum.IsDefined(typeof(PaymentProvider), paymentProvider)
    )
    {
      return new ValidationResult(
        $"Invalid payment provider. Payment providers can only be {string.Join(",", providers)}"
      );
    }
    return ValidationResult.Success;
  }
}
