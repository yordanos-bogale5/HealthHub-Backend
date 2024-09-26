using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Dtos;
using Microsoft.AspNetCore.Antiforgery;
using Xunit;

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

public class CreateConversationDto
{
  [Required]
  [GuidCollection]
  public required ICollection<Guid> Participants = [];
}
