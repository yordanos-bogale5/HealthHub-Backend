using System.Text.Json;
using HealthHub.Source.Helpers.Defaults;

public class CookieMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext context)
  {
    var accessToken = context.Request.Cookies[AuthDefaults.AccessToken];

    if (!string.IsNullOrEmpty(accessToken))
    {
      // Set the token in the Authorization header
      context.Request.Headers[AuthDefaults.Authorization] = $"Bearer {accessToken}";
    }
    await next(context);
  }
}
