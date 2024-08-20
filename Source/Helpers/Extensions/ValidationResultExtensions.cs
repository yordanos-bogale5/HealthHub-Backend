using FluentValidation.Results;

public static class ValidationResultExtensions
{
  public static Dictionary<string, string[]> ToFluentValidationErrorResult(
    this ValidationResult validationResult
  )
  {
    // Group errors by property name and aggregate messages into an array
    return validationResult
      .Errors.GroupBy(err => err.PropertyName)
      .ToDictionary(group => group.Key, group => group.Select(err => err.ErrorMessage).ToArray());
  }
}
