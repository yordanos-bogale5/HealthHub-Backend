using System.ComponentModel.DataAnnotations;

public class GuidAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
    {
      return new ValidationResult("Guid cannot be null");
    }

    if (!Guid.TryParse(value.ToString(), out Guid parsed) || parsed == Guid.Empty)
    {
      return new ValidationResult("Value is not a valid guid string.");
    }

    return ValidationResult.Success;
  }
}
