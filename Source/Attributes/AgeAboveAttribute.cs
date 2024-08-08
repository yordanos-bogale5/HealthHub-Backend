using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Attributes;

public class AgeAboveAttributes(int minAge = 18, string errMsg = "You must be at least 18 years old.") : ValidationAttribute
{
  public override bool IsValid(object? value)
  {
    if (value is DateTime birthDate)
    {
      var age = DateTime.Today.Year - birthDate.Year;
      if (birthDate > DateTime.Today.AddYears(-age)) age--; // Subtract 1 if birthday hasn't occurred yet
      return age >= minAge;
    }
    return false;
  }

  public override string FormatErrorMessage(string name)
  {
    return errMsg;
  }
}

