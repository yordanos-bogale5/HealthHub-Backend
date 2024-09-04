using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Auth0.AspNetCore.Authentication.BackchannelLogout;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Cmp;

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
      await ExceptionHandler.HandleBadHttpRequestAsync(httpContext, ex);
    }
    catch (KeyNotFoundException ex)
    {
      await ExceptionHandler.HandleKeyNotFoundAsync(httpContext, ex);
    }
    catch (UnauthorizedAccessException ex)
    {
      await ExceptionHandler.HandleUnauthorizedAccess(httpContext, ex);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Caught exception of type: {ex.GetType()}");
      await ExceptionHandler.HandleInternalException(httpContext, ex);
    }
  }
}

internal static class Messages
{
  internal static Dictionary<string, string> RegisterModelErrorMessages =
    new()
    {
      { "Role", "Invalid Role Provided. Role can be only Patient, Doctor or Admin" },
      { "Gender", "Invalid Gender Provided. Gender can be only Male or Female." },
    };
}

internal static class ExceptionHandler
{
  internal static async Task HandleBadHttpRequestAsync(
    HttpContext httpContext,
    BadHttpRequestException ex
  )
  {
    httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

    object errors = new { };

    // Map Errors Correctly for Model State Errors
    if (httpContext.Items.ContainsKey(ErrorFieldConstants.ModelStateErrors))
    {
      var data = (ModelStateDictionary)httpContext.Items[ErrorFieldConstants.ModelStateErrors]!;

      errors = data.ToDictionary(
        kvp => kvp.Key,
        kvp =>
          kvp.Value?.Errors.Select(e =>
              Messages.RegisterModelErrorMessages.TryGetValue(kvp.Key, out string? value)
                ? value
                : e.ErrorMessage
            )
            .ToList()
      );
    }
    // Map Errors Correctly for Fluent Validation Errors
    else if (httpContext.Items.ContainsKey(ErrorFieldConstants.FluentValidationErrors))
    {
      errors =
        (IDictionary<string, string[]>)
          httpContext.Items[ErrorFieldConstants.FluentValidationErrors]!;
    }

    await httpContext.Response.WriteAsync(
      JsonSerializer.Serialize(new ErrorResponse() { title = ex.Message, errors = errors })
    );
  }

  internal static async Task HandleKeyNotFoundAsync(
    HttpContext httpContext,
    KeyNotFoundException ex
  )
  {
    httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

    object errors = new { };

    await httpContext.Response.WriteAsync(
      JsonSerializer.Serialize(new ErrorResponse() { title = ex.Message, errors = errors })
    );
  }

  internal static async Task HandleUnauthorizedAccess(
    HttpContext httpContext,
    UnauthorizedAccessException ex
  )
  {
    httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

    object errors = new { };

    await httpContext.Response.WriteAsync(
      JsonSerializer.Serialize(new ErrorResponse() { title = ex.Message, errors = errors })
    );
  }

  internal static async Task HandleInternalException(HttpContext httpContext, Exception ex)
  {
    httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

    var errorResponse = new { title = "An unexpected error occurred.", errors = ex.Message };

    await httpContext.Response.WriteAsync(
      JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true })
    );
  }
}
