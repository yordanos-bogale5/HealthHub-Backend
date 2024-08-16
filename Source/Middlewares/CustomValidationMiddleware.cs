using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Middlewares;

public class CustomValidationMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext httpContext)
  {
    await next(httpContext); // Let other functions in the req pipeline be called

    foreach (var kvp in httpContext.Items)
      Console.WriteLine($"\n\n{kvp.Key} : {kvp.Value}\n\n");


    // If the response is a bad request and there are fluent validation errors present
    if (
        httpContext.Response.StatusCode == StatusCodes.Status400BadRequest &&
        httpContext.Items.ContainsKey("FluentValidationErrors"))
    {
      var errors = (IDictionary<string, string[]>)httpContext.Items["FluentValidationErrors"]!;

      var problemDetails = new ValidationProblemDetails(errors)
      {
        Title = "One or more validation errors occured!",
        Status = StatusCodes.Status400BadRequest,
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
      };

      httpContext.Response.ContentType = "application/problem+json";

      await httpContext.Response.WriteAsJsonAsync(problemDetails); // Write the formatted Fluent Error to the response context
      Console.WriteLine($"\n\nProblemDetails: \n{problemDetails}\n");
    }
  }
}