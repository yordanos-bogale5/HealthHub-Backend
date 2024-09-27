using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Payment : BaseEntity
{
  public Guid PaymentId { get; set; } = Guid.NewGuid();

  // Used for verification purposes between the payment provider and the application
  public required string TransactionReference { get; set; }

  // The below fks are nullable because we don't want cascade to happen on payment
  // records. Instead what happens is that these keys assume a null value
  // Keeping only the essential information listed below intact even when a user deletion happens.

  public Guid? SenderId { get; set; } // <<FK>>

  public Guid? ReceiverId { get; set; } // <<FK>>

  // User details (to prevent loss on user deletion)
  public required string SenderName { get; set; }
  public required string SenderEmail { get; set; }
  public required string ReceiverName { get; set; }
  public required string ReceiverEmail { get; set; }

  public required decimal Amount { get; set; }

  public required PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // Pending, Success, Failed etc..

  public required PaymentProvider PaymentProvider { get; set; } // Chapa, Paypal etc..

  public required PaymentType PaymentType { get; set; } // Transfer, Charge etc..

  public virtual User? Sender { get; set; } // <<NAV>>
  public virtual User? Receiver { get; set; } // <<NAV>>
}
