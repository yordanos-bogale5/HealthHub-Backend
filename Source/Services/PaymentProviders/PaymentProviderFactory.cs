using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;

public class PaymentProviderFactory(IEnumerable<IPaymentProvider> paymentProviders)
  : IPaymentProviderFactory
{
  /// <summary>
  /// Get payment provider
  /// </summary>
  /// <param name="paymentProvider"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public IPaymentProvider GetProvider(PaymentProvider paymentProvider)
  {
    return paymentProviders.FirstOrDefault(x => x.PaymentProvider == paymentProvider)
      ?? throw new ArgumentException("Payment provider not found!");
  }
}
