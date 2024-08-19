using FluentValidation.Results;

public static class ValidationResultExtensions
{
    public static Dictionary<string, string[]> ToFluentValidationErrorResult(
        this ValidationResult validationResult
    )
    {
        return validationResult.Errors.ToDictionary(
            err => err.PropertyName,
            err => new[] { err.ErrorMessage }
        );
    }
}
