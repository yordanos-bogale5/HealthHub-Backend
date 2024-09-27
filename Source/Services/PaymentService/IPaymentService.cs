using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces.Payments;

namespace HealthHub.Source.Services.PaymentService;

public interface IPaymentService
{
  Task<decimal> CheckBalanceAsync(string email, PaymentProvider provider);

  Task<TransferResponseDto> TransferAsync(TransferRequestDto transferRequestDto, Guid senderId);

  Task<PaymentDto> CreatePaymentAsync(
    CreatePaymentDto createPaymentDto,
    string transactionReference
  );

  Task<ChargeResponse> ChargeAsync(IChargeRequest charge);

  Task<PaymentDto> ChangePaymentStatusAsync(string transactionReference, PaymentStatus status);

  Task<IVerifyResponse> VerifyAsync(IVerifyRequest verifyRequest);
}
