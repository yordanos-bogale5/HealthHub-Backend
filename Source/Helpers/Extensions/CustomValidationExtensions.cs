using HealthHub.Source.Middlewares;

namespace HealthHub.Source.Helpers.Extensions;

public static class CustomValidationMiddlewareExtensions {
  public static IApplicationBuilder UseCustomValidation(this IApplicationBuilder applicationBuilder) {
    return applicationBuilder.UseMiddleware<CustomValidationMiddleware>();
  }
}
