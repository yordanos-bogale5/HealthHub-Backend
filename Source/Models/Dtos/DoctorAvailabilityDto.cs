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

public record AvailabilityDto(string Day, string StartTime, string EndTime);
