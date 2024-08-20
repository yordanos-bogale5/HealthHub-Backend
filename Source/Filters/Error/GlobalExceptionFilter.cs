using System.Text.Json;
using HealthHub.Source.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthHub.Source.Filters.Error;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter {
  public void OnException(ExceptionContext context) {
    logger.LogError(context.Exception, "Unhandled exception occured!");

    ObjectResult result;

    Console.WriteLine($"\n\nResponse is: ${context.HttpContext.Response.ToString()}");

    if (context.Exception is BadHttpRequestException) {
      result = new ObjectResult(
          new { title = "Bad request error.", errors = context.Exception.Data }
      ) {
        StatusCode = StatusCodes.Status400BadRequest
      };
    } else if (context.Exception is JsonException) {
      var validationErrors = context
          .ModelState.Where(ms => ms.Value!.Errors.Count > 0)
          .ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
          );

      result = new ObjectResult(
          new { message = ErrorMessages.ModelValidationError, errors = validationErrors }
      );
    } else if (context.HttpContext.Items.ContainsKey("FluentValidationErrors")) {
      var fluentResult =
          (IDictionary<string, string[]>)context.HttpContext.Items["FluentValidationErrors"]!;
      result = new ObjectResult(
          new { message = ErrorMessages.ModelValidationError, errors = fluentResult }
      ) {
        StatusCode = StatusCodes.Status400BadRequest
      };
    } else if (context.Exception is JsonException) {
      var validationErrors = context
          .ModelState.Where(ms => ms.Value!.Errors.Count > 0)
          .ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
          );

      result = new ObjectResult(
          new { message = context.HttpContext.Response.Body, errors = validationErrors }
      ) {
        StatusCode = StatusCodes.Status400BadRequest // Set the status code to 400 Bad Request
      };
    } else {
      result = new ObjectResult(
          new {
            message = "An error occurred while processing your request. Please try again later.",
            errors = context.Exception.Message
          }
      ) {
        StatusCode = StatusCodes.Status500InternalServerError
      };
    }

    context.Result = result;
    context.ExceptionHandled = true;
  }
}
