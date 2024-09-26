using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

public class PaymentCurrencyAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var validCurrencies = Enum.GetNames<PaymentCurrency>();
    try
    {
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
      {
        return new ValidationResult("Currency is required");
      }

      if (
        Enum.TryParse<PaymentCurrency>(value.ToString(), out var currency)
        && currency != PaymentCurrency.Unknown
      )
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(
        $"Invalid currency. Currency can only be {string.Join(",", validCurrencies.Skip(1))}"
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine($"{ex}");

      return new ValidationResult(
        $"Invalid currency. Currency can only be {string.Join(",", validCurrencies.Skip(1))}"
      );
    }
  }
}
