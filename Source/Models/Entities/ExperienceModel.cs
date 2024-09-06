using HealthHub.Source.Models.Entities;

public class Experience
{
  public Guid ExperienceId { get; set; } = Guid.NewGuid();
  public required string Institution { get; set; }

  public required DateOnly StartDate { get; set; }
  public required DateOnly? EndDate { get; set; } // Nullable (if null then still working in the institute)

  public string? Description { get; set; }

  public required Guid DoctorId { get; set; }
  public virtual Doctor? Doctor { get; set; } // nav
}
