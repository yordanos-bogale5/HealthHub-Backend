using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Interfaces;

public class TimeRange(TimeOnly startTime, TimeOnly endTime)
{
  public TimeOnly StartTime { get; set; } = startTime;
  public TimeOnly EndTime { get; set; } = endTime;

  public void Deconstruct(out TimeOnly startTime, out TimeOnly endTime)
  {
    startTime = StartTime;
    endTime = EndTime;
  }
}
