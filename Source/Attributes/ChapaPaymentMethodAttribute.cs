using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Interfaces.Payments.Chapa;

public class ChapaPaymentMethodAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    List<string> validPaymentMethods = new();
    foreach (var property in typeof(ChapaPaymentMethod).GetFields())
    {
      if (property.GetValue(null) != null)
        validPaymentMethods.Add((string)property.GetValue(null)!);
    }
    try
    {
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
      {
        return new ValidationResult("Payment method is required");
      }

      if (validPaymentMethods.Any(e => e.Equals(value.ToString())))
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(
        $"Invalid payment method. Payment method can only be {string.Join(",", validPaymentMethods)}"
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine($"{ex}");

      return new ValidationResult(
        $"Invalid payment method. Payment method can only be {string.Join(",", validPaymentMethods)}"
      );
    }
  }
}
