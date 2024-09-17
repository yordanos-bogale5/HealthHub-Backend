using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payments")]
public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
  : ControllerBase
{
  [HttpGet("balance/{email}/{paymentProvider}")]
  public async Task<IActionResult> CheckBalance(
    [FromRoute] string email,
    [FromRoute] [PaymentProvider] PaymentProvider paymentProvider
  )
  {
    try
    {
      var balance = await paymentService.CheckBalanceAsync(email, paymentProvider);
      return Ok(balance);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error checking balance");
      throw;
    }
  }

  [HttpPost("transfer")]
  public async Task<IActionResult> TransferBalance([Required] TransferRequestDto transferRequestDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var userId = Request.Cookies[CookieDefaults.Profile.UserId]?.ToString();

      if (string.IsNullOrEmpty(userId))
      {
        throw new ArgumentException(
          "Either you haven't logged in or the cookie for userId is missing."
        );
      }

      if (!Guid.TryParse(userId, out var senderId))
      {
        throw new FormatException("Invalid userId format. Please login again.");
      }

      var result = await paymentService.TransferAsync(transferRequestDto, senderId);
      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error checking balance");
      throw;
    }
  }
}
