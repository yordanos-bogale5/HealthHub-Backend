using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Services.PaymentService;

public interface IPaymentService
{
  Task<decimal> CheckBalanceAsync(string email, PaymentProvider provider);
  Task<TransferResponseDto> TransferAsync(TransferRequestDto transferRequestDto, Guid senderId);
  Task<PaymentDto> CreatePaymentAsync(
    CreatePaymentDto createPaymentDto,
    string transactionReference
  );
}
