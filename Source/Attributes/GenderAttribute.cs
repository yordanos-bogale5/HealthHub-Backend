using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes
{
  public class GenderAttribute : ValidationAttribute
  {
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
      if (value == null)
      {
        return new ValidationResult("Gender field cannot be empty.");
      }

      var gender = value.ToString();

      if (gender != "Male" && gender != "Female")
      {
        return new ValidationResult("Gender must be either Male or Female.");
      }

      return ValidationResult.Success;
    }
  }
}
