using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;

public class ChapaPaymentProvider : IPaymentProvider
{
  public PaymentProvider PaymentProvider => PaymentProvider.Chapa;

  public Task<decimal> CheckBalanceAsync(string email) => throw new NotImplementedException();

  public Task<bool> TransferAsync(string senderEmail, string receiverEmail, decimal amount) =>
    throw new NotImplementedException();
}
