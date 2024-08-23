using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Models.Dtos;

public record Availability
{
  public required Guid DoctorAvailabilityId { get; init; }
  public required Days Day { get; init; }
  public required List<TimeRange> AvailableTimes { get; init; }
}

public record AvailabilityDto
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
