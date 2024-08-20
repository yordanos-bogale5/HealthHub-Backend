using HealthHub.Source.Models.Enums;

public record DoctorAvailabilityDto {
  public required Guid DoctorAvailabilityId { get; set; }
  public required Guid DoctorId { get; set; }
  public required Days AvailableDay { get; set; }
  public required TimeOnly StartTime { get; set; }
  public required TimeOnly EndTime { get; set; }
}
