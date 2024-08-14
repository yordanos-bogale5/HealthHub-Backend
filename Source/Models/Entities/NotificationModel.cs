using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Notification
{
  public Guid NotificationId { get; set; } = Guid.NewGuid();
  [Required]
  public required Guid UserId { get; set; } // <<FK>>
  [Required]
  public required NotificationType NotificationType { get; set; }
  [Required]
  public required string Message { get; set; }

  public virtual User? User { get; set; }

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}