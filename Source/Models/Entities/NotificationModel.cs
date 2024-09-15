using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Notification : BaseEntity
{
  public Guid NotificationId { get; set; } = Guid.NewGuid();

  [Required]
  public required Guid UserId { get; set; } // <<FK>>

  [Required]
  public required NotificationType NotificationType { get; set; }

  [Required]
  public required string Message { get; set; }

  public virtual User? User { get; set; }
}
