namespace HealthHub.Source.Models.Entities;

public class Education
{
  public Guid EducationId { get; set; } = Guid.NewGuid();

  public required string Degree { get; set; }
  public required string Institution { get; set; }

  public required DateOnly StartDate { get; set; }
  public required DateOnly EndDate { get; set; }

  public required Guid DoctorId { get; set; }
  public virtual Doctor? Doctor { get; set; } // Nav
}
