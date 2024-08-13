using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using HealthHub.Source.Enums;

namespace HealthHub.Source.Models.Entities;

public class Message
{
  public Guid MessageId { get; set; } = Guid.NewGuid(); // <<PK>>
  [Required]
  public required Guid ChatId { get; set; } // <<FK>>
  [Required]
  public required Guid FileId { get; set; } // <<FK>>
  public string? MessageText { get; set; }

  public virtual Chat? Chat { get; set; }
  public virtual File? File { get; set; }
}