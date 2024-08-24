using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class SchedulingService(ApplicationContext appContext, ILogger<SchedulingService> logger)
{
  public async Task<Dictionary<Days, List<TimeRange>>> GetDoctorAppointmentTimesAsync(Guid doctorId)
  {
    try
    {
      var docAppTimes = new Dictionary<Days, List<TimeRange>>();

      var appointments = await appContext
        .Appointments.Where(ap => ap.DoctorId == doctorId)
        .ToListAsync();

      foreach (var ap in appointments)
      {
        Days day = ap.AppointmentDate.DayOfWeek.ToString().ConvertToEnum<Days>();
        if (!docAppTimes.ContainsKey(day))
          docAppTimes[day] = new List<TimeRange>();
        Console.WriteLine(day);
        docAppTimes[day]
          .Add(new TimeRange(ap.AppointmentTime, ap.AppointmentTime.Add(ap.AppointmentTimeSpan)));
      }

      return docAppTimes;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An Error occured trying to get doctor appointment times");
      throw;
    }
  }
}
