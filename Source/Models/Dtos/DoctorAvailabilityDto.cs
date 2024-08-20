using HealthHub.Source.Models.Enums;
using Microsoft.AspNetCore.Mvc;

public record DoctorAvailabilityDto
{
  public required Guid DoctorAvailabilityId { get; set; }
  public required Guid DoctorId { get; set; }
  public required Days AvailableDay { get; set; }
  public required TimeOnly StartTime { get; set; }
  public required TimeOnly EndTime { get; set; }
}

public record Availability
{
  public required string Day { get; init; }
  public required string StartTime { get; init; }
  public required string EndTime { get; init; }

  public void Deconstruct(out string day, out string startTime, out string endTime)
  {
    day = Day;
    startTime = StartTime;
    endTime = EndTime;
  }
}
