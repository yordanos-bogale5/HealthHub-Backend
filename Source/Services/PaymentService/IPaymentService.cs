using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Services.PaymentService;

public interface IPaymentService
{
  Task<decimal> CheckBalanceAsync(string email, PaymentProvider provider);
  Task<bool> TransferAsync(
    string senderEmail,
    string receiverEmail,
    decimal amount,
    PaymentProvider provider
  );
}
