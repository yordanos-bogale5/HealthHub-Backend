using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Conversation : BaseEntity
{
  public Guid ConversationId { get; set; } = Guid.NewGuid(); // <<PK>>
  public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
  public virtual ICollection<ConversationMembership> ConversationMemberships { get; set; } =
    new HashSet<ConversationMembership>();
}
