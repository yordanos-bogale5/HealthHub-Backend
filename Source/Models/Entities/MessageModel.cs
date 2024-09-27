using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Message : BaseEntity
{
  public Guid MessageId { get; set; } = Guid.NewGuid(); // <<PK>>
  public string? MessageText { get; set; }
  public virtual ICollection<File>? Files { get; set; } = new HashSet<File>();

  public Guid SenderId { get; set; }
  public virtual User? Sender { get; set; }

  public Guid ConversationId { get; set; }
  public virtual Conversation? Conversation { get; set; }
}
