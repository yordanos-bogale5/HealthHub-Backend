using HealthHub.Source.Middlewares;

namespace HealthHub.Source.Helpers.Extensions;

public static class MiddlewareExtensions
{
  public static IApplicationBuilder UseCustomValidation(this IApplicationBuilder applicationBuilder)
  {
    return applicationBuilder.UseMiddleware<CustomValidationMiddleware>();
  }

  public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder applicationBuilder)
  {
    return applicationBuilder.UseMiddleware<CookieMiddleware>();
  }
}
