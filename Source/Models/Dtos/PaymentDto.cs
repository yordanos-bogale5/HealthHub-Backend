using System.ComponentModel.DataAnnotations;
using Flurl.Util;
using HealthHub.Source.Models.Enums;

public record TransferRequestDto
{
  [Required]
  public required string SenderName { get; set; }

  /// <summary>
  /// Email address of the patient sending the funds.
  /// </summary>
  [Required]
  [EmailAddress]
  public required string SenderEmail { get; set; }

  [Required(ErrorMessage = "Receiver name is required for verification purposes.")]
  public required string ReceiverName { get; set; }

  [Required]
  public required string ReceiverEmail { get; set; }

  [Required]
  [Phone]
  public required string PhoneNumber { get; set; }

  [Required]
  [Guid]
  public required Guid ReceiverId { get; set; }

  /// <summary>
  /// Amount of funds to transfer.
  /// </summary>
  [Required]
  [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0.01")]
  public required decimal Amount { get; set; }

  /// <summary>
  /// Payment provider used for the transaction.
  /// </summary>
  [Required]
  [PaymentProvider]
  public required PaymentProvider PaymentProvider { get; set; }
}

public interface ITransferResponseDto
{
  public bool IsSuccessful { get; set; }
  public object Message { get; set; }
}

public class TransferResponseDto : ITransferResponseDto
{
  public required bool IsSuccessful { get; set; }
  public required object Message { get; set; }
};

public interface ITransactionReferable
{
  public string TransactionReference { get; set; }
}

public class TransferResponseInner : ITransferResponseDto, ITransactionReferable
{
  public required bool IsSuccessful { get; set; }
  public required object Message { get; set; }
  public required string TransactionReference { get; set; }
};

// This is what is returned to the client
public record PaymentDto
{
  public required Guid PaymentId { get; set; }
  public required Guid? SenderId { get; set; }
  public required Guid? ReceiverId { get; set; }

  public required string SenderName { get; set; }
  public required string SenderEmail { get; set; }
  public required string ReceiverName { get; set; }
  public required string ReceiverEmail { get; set; }

  public required decimal Amount { get; set; }
  public required PaymentStatus PaymentStatus { get; set; }
  public required PaymentProvider PaymentProvider { get; set; }
}

public record CreatePaymentDto
{
  public required Guid SenderId { get; set; }
  public required Guid ReceiverId { get; set; }

  public required string SenderName { get; set; }
  public required string SenderEmail { get; set; }
  public required string ReceiverName { get; set; }
  public required string ReceiverEmail { get; set; }

  public required decimal Amount { get; set; }
  public required PaymentStatus PaymentStatus { get; set; }
  public required PaymentProvider PaymentProvider { get; set; }
}
