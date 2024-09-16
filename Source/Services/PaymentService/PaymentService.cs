using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;

namespace HealthHub.Source.Services.PaymentService;

public class PaymentService(
  IPaymentProviderFactory paymentProviderFactory,
  ILogger<PaymentService> logger
) : IPaymentService
{
  public async Task<decimal> CheckBalanceAsync(string email, PaymentProvider provider)
  {
    try
    {
      IPaymentProvider paymentProvider = paymentProviderFactory.GetProvider(provider);
      var balance = await paymentProvider.CheckBalanceAsync(email);
      return balance;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error checking balance");
      throw;
    }
  }

  public async Task<bool> TransferAsync(
    string senderEmail,
    string receiverEmail,
    decimal amount,
    PaymentProvider provider
  )
  {
    try
    {
      IPaymentProvider paymentProvider = paymentProviderFactory.GetProvider(provider);
      var result = await paymentProvider.TransferAsync(senderEmail, receiverEmail, amount);
      return result;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error transferring funds");
      throw;
    }
  }
}
