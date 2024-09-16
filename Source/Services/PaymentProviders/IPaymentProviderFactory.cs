using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;

public interface IPaymentProviderFactory
{
  IPaymentProvider GetProvider(PaymentProvider paymentProvider);
}
