using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class GuidCollectionAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
    {
      return new ValidationResult("Guids cannot be null");
    }

    if (value is not IEnumerable<Guid> values)
    {
      return new ValidationResult("Input must be a collection of GUIDs.");
    }

    foreach (var val in values)
    {
      if (val == Guid.Empty)
      {
        return new ValidationResult($"Value {val} is an empty GUID.");
      }
    }

    if (values.Count() == 0)
    {
      return new ValidationResult("Collection cannot be empty.");
    }

    return ValidationResult.Success;
  }
}
