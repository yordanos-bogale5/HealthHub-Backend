using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Payment : BaseEntity
{
  public Guid PaymentId { get; set; } = Guid.NewGuid();

  public required string TransactionReference { get; set; } // Used for verification purposes between the payment provider and the application

  [Required]
  public required Guid SenderId { get; set; } // <<FK>>

  [Required]
  public required Guid ReceiverId { get; set; } // <<FK>>

  [Required]
  public required decimal Amount { get; set; }

  [Required]
  public required PaymentStatus PaymentStatus { get; set; }

  [Required]
  public required PaymentProvider PaymentProvider { get; set; } = PaymentProvider.Chapa;

  public virtual User? Sender { get; set; } // <<NAV>>
  public virtual User? Receiver { get; set; } // <<NAV>>
}
