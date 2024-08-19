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
    Dictionary<string, string> RegisterModelErrorMessages =
        new()
        {
            { "Role", "Invalid Role Provided. Role can be only Patient, Doctor or Admin" },
            { "Gender", "Invalid Gender Provided. Gender can be only Male or Female." },
        };

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
            if (httpContext.Items.ContainsKey(ErrorFieldConstants.ModelStateErrors))
            {
                var data = (ModelStateDictionary)
                    httpContext.Items[ErrorFieldConstants.ModelStateErrors]!;
                errors = data.ToDictionary(
                    kvp => kvp.Key,
                    kvp =>
                        kvp.Value?.Errors.Select(e =>
                                RegisterModelErrorMessages.TryGetValue(kvp.Key, out string? value)
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
                JsonSerializer.Serialize(
                    new ErrorResponse() { title = ex.Message, errors = errors }
                )
            );
        }
        catch (KeyNotFoundException ex)
        {
            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            object errors = new { };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new ErrorResponse() { title = ex.Message, errors = errors }
                )
            );
        }
        catch (Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new
            {
                title = "An unexpected error occurred.",
                errors = ex.Message
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(
                    errorResponse,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
        }
    }
}
