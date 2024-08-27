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

  public static string ToSnakeCase(this string str)
  {
    string snake_str = "";
    foreach (char c in str)
    {
      if (char.IsWhiteSpace(c))
      {
        snake_str += '_';
      }
      else if (char.IsUpper(c))
      {
        // Only add an underscore if it's not the first character
        if (snake_str.Length > 0)
        {
          snake_str += '_';
        }
        snake_str += char.ToLower(c);
      }
      else
      {
        snake_str += c;
      }
    }
    return snake_str.Trim('_'); // Trim any leading or trailing underscores
  }
}
