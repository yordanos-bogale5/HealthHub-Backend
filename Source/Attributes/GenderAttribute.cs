using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

public class GenderAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    try
    {
      if (value == null)
      {
        return new ValidationResult("Gender field cannot be empty.");
      }

      if (value is not Gender gender)
      {
        return new ValidationResult("Invalid gender value.");
      }

      if (!Enum.IsDefined(typeof(Gender), gender))
      {
        return new ValidationResult("Gender must be either Male or Female.");
      }

      return ValidationResult.Success;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"{ex}");
      return new ValidationResult("Invalid Gender type! Must be either Male or Female!");
    }
  }
}
