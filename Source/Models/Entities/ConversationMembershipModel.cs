using HealthHub.Source.Models.Entities;

public class ConversationMembership
{
  public required Guid UserId { get; set; }
  public virtual User? User { get; set; }

  public required Guid ConversationId { get; set; }
  public virtual Conversation? Conversation { get; set; }
}
