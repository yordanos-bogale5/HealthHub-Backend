using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Chat
{
  public Guid ChatId { get; set; } = Guid.NewGuid(); // <<PK>>
  [Required]
  public required Guid SenderId { get; set; } // <<FK>>
  [Required]
  public required Guid ReceiverId { get; set; } // <<FK>>

  public virtual User? Sender { get; set; }
  public virtual User? Receiver { get; set; }
}