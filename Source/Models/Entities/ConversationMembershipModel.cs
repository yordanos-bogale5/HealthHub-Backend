using HealthHub.Source.Models.Entities;

public class ConversationMembership
{
  public Guid UserId { get; set; }
  public virtual User? User { get; set; }

  public Guid ConversationId { get; set; }
  public virtual Conversation? Conversation { get; set; }
}
