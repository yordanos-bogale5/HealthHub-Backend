using System.Text.Json;
using System.Text.Json.Nodes;
using Auth0.AspNetCore.Authentication.BackchannelLogout;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HealthHub.Source.Middlewares;

public class CustomValidationMiddleware(RequestDelegate next) : ControllerBase
{
  public async Task InvokeAsync(HttpContext httpContext)
  {
    try
    {
      await next(httpContext); // Let other functions in the req pipeline be called
    }
    catch (BadHttpRequestException ex)
    {
      httpContext.Response.ContentType = "application/problem+json";
      httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

      object errors = new { };

      // Map Errors Correctly for Model State Errors
      if (httpContext.Items.ContainsKey(ErrorConstants.ModelStateErrors))
      {
        var data = (ModelStateDictionary)httpContext.Items[ErrorConstants.ModelStateErrors]!;
        errors = data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList());
      }
      // Map Errors Correctly for Fluent Validation Errors
      else if (httpContext.Items.ContainsKey(ErrorConstants.FluentValidationErrors))
      {
        errors = (IDictionary<string, string[]>)httpContext.Items[ErrorConstants.FluentValidationErrors]!;
      }

      await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse()
      {
        title = ex.Message,
        errors = errors
      }));

    }
    catch (Exception ex)
    {
      httpContext.Response.ContentType = "application/json";
      httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

      var errorResponse = new
      {
        message = "An unexpected error occurred.",
        details = ex.Message
      };

      await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true }));
    }
  }
}