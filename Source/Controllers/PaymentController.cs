using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Interfaces.Payments;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services.PaymentService;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/payments")]
public class PaymentController(
  IPaymentService paymentService,
  ILogger<PaymentController> logger,
  AppConfig appConfig
) : ControllerBase
{
  private string requestBody;

  // [HttpGet("balance/{email}/{paymentProvider}")]
  // public async Task<IActionResult> CheckBalance(
  //   [FromRoute] string email,
  //   [FromRoute] [PaymentProvider] PaymentProvider paymentProvider
  // )
  // {
  //   try
  //   {
  //     var balance = await paymentService.CheckBalanceAsync(email, paymentProvider);
  //     return Ok(balance);
  //   }
  //   catch (System.Exception ex)
  //   {
  //     logger.LogError(ex, "Error checking balance");
  //     throw;
  //   }
  // }

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

  [HttpPost("charge")]
  public async Task<IActionResult> ChargeCustomer([Required] ChargeRequest chargeRequest)
  {
    try
    {
      var result = await paymentService.ChargeAsync(chargeRequest);
      return Ok(new ApiResponse<ChargeResponse>(result.Status, result.Message, result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error charging customer");
      throw;
    }
  }

  [HttpPost("chapa/callback")]
  public async Task<IActionResult> ChapaCallback([FromBody] string status, [FromBody] string tx_rf)
  {
    try
    {
      // var result = await paymentService.VerifyAsync(
      //   new VerifyRequest { TransactionReference = tx_rf }
      // );

      var result = await Task.FromResult("");

      return Ok(new { status, tx_rf });
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error while processing chapa callback");
      throw;
    }
  }

  /// <summary>
  /// For intergration with Chapa payment provider, please refer to the documentation
  /// ref: https://developer.chapa.co/integrations/webhooks
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  [HttpPost("chapa/webhooks")]
  public async Task<IActionResult> ChapaPaymentWebhooks([FromBody] object body)
  {
    try
    {
      var chapaSignature = Request.Headers["Chapa-Signature"].ToString();
      var requestBody = JsonConvert.SerializeObject(body);

      var hash = EncryptionHelper.GetHmacSha256Hash(
        requestBody,
        appConfig.WebhookSecret
          ?? throw new Exception("No secret key. Please check the configuration.")
      );

      if (hash != chapaSignature)
      {
        throw new ArgumentException(
          $"Invalid signature.\nHash: {hash}\nSignature: {chapaSignature}"
        );
      }

      return Ok();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error while checking payment webhooks.");
      throw;
    }
  }
}
