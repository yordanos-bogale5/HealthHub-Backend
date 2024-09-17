using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;

namespace HealthHub.Source.Services.PaymentService;

public class PaymentService(
  IPaymentProviderFactory paymentProviderFactory,
  DoctorService doctorService,
  PatientService patientService,
  ApplicationContext appContext,
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

  public async Task<TransferResponseDto> TransferAsync(
    TransferRequestDto transferRequestDto,
    Guid senderId
  )
  {
    try
    {
      IPaymentProvider paymentProvider = paymentProviderFactory.GetProvider(
        transferRequestDto.PaymentProvider
      );

      // Perform the transfer
      var result = await paymentProvider.TransferAsync(transferRequestDto);

      // Create a payment record
      var payment = await CreatePaymentAsync(
        transferRequestDto.ToCreatePaymentDto(senderId, result.IsSuccessful),
        result.TransactionReference
      );

      // TODO

      return new TransferResponseDto
      {
        IsSuccessful = result.IsSuccessful,
        Message = new { result.Message, payment }
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error transferring funds");
      throw;
    }
  }

  public async Task<PaymentDto> CreatePaymentAsync(
    CreatePaymentDto createPaymentDto,
    string transactionReference
  )
  {
    try
    {
      if (!await doctorService.UserExistsAsync(createPaymentDto.ReceiverId))
      {
        throw new ArgumentException("Doctor with the specified receiver_id does not exist");
      }

      if (!await patientService.UserExistsAsync(createPaymentDto.SenderId))
      {
        throw new ArgumentException("Patient with the specified user_id does not exist");
      }

      var payment = createPaymentDto.ToPayment(transactionReference);
      var result = await appContext.Payments.AddAsync(payment);
      await appContext.SaveChangesAsync();
      return result.Entity.ToPaymentDto();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error creating payment");
      throw;
    }
  }
}
