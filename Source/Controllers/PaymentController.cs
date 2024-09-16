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
}
