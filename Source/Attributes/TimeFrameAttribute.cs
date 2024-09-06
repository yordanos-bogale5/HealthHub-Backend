using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

public class TimeFrameAttribute() : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
      return ValidationResult.Success;

    if (
      value is not TimeFrame timeFrame
      || (timeFrame != TimeFrame.Day && timeFrame != TimeFrame.Month && timeFrame != TimeFrame.Year)
    )
    {
      return new ValidationResult(
        $"Its not a valid timeFrame. Time frame should only be {string.Join(", ", Enum.GetNames<TimeFrame>())}"
      );
    }

    return ValidationResult.Success;
  }
}
