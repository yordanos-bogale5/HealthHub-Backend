using System.Net;
using HealthHub.Source.Helpers.Defaults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (!context.ModelState.IsValid)
    {
      // Store the ModelState errors in HttpContext.Items
      context.HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = context.ModelState;

      throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
    }
  }
}
