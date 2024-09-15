namespace HealthHub.Source.Models.Entities;

public class BaseEntity
{
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
