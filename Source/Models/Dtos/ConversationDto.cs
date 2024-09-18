using HealthHub.Source.Models.Dtos;

public interface IConversationDto
{
  Guid ConversationId { get; set; }
  ICollection<IProfileDto> Participants { get; set; }
}

public class ConversationDtoBase : IConversationDto
{
  public required Guid ConversationId { get; set; }
  public required ICollection<IProfileDto> Participants { get; set; }
}
