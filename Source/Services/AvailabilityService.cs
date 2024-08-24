using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class AvailabilityService(
  ApplicationContext appContext,
  SchedulingService schedulingService,
  DoctorService doctorService,
  ILogger<AvailabilityService> logger
)
{
  /// <summary>
  /// Creates doctor availability entries given a List of tuple having (Day, StartTime, EndTime)
  /// </summary>
  /// <param name="doctorAvailabilities"></param>
  /// <param name="doctor"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<List<DoctorAvailability>>> AddDoctorAvailabilityAsync(
    List<AvailabilityDto> doctorAvailabilities,
    Doctor doctor
  )
  {
    try
    {
      List<DoctorAvailability> dbDoctorAvailabilities = [];

      if (doctorAvailabilities.Count == 0)
      {
        foreach (
          Days day in new List<Days>
          {
            Days.Monday,
            Days.Tuesday,
            Days.Wednesday,
            Days.Thursday,
            Days.Friday
          }
        )
        {
          dbDoctorAvailabilities.Add(
            new DoctorAvailability
            {
              Doctor = doctor,
              DoctorId = doctor.DoctorId,
              AvailableDay = day,
              StartTime = new TimeOnly(10, 0),
              EndTime = new TimeOnly(17, 0)
            }
          );
        }
      }
      else
      {
        foreach (var (day, startTime, endTime) in doctorAvailabilities)
        {
          dbDoctorAvailabilities.Add(
            new DoctorAvailability
            {
              Doctor = doctor,
              DoctorId = doctor.DoctorId,
              AvailableDay = day.ConvertToEnum<Days>(),
              StartTime = TimeOnly.Parse(startTime),
              EndTime = TimeOnly.Parse(endTime)
            }
          );
        }
      }

      await appContext.DoctorAvailabilities.AddRangeAsync(dbDoctorAvailabilities);
      await appContext.SaveChangesAsync();

      return new ServiceResponse<List<DoctorAvailability>>
      {
        Data = dbDoctorAvailabilities,
        Message = "Doctor availabilities created successfully",
        StatusCode = 204,
        Success = true
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured when trying to add doctor availability {ex}");
      throw new Exception("An error occured when trying to add doctor availability");
    }
  }

  /// <summary>
  /// To Check if a doctor is available at the appointmentDay and Time
  /// </summary>
  /// <param name="doctorId"></param>
  /// <param name="appointmentDay"></param>
  /// <param name="appointmentTime"></param>
  /// <param name="appointmentTimeSpan"></param>
  /// <returns>True if he/she is available, otherwise false.</returns>
  public async Task<bool> CheckDoctorAvailabilityAsync(
    Guid doctorId,
    Days appointmentDay,
    TimeOnly appointmentTime,
    TimeSpan appointmentTimeSpan
  )
  {
    try
    {
      var result = await appContext.DoctorAvailabilities.ToListAsync();
      return result.Any(da =>
        da.DoctorId == doctorId
        && da.AvailableDay == appointmentDay
        && da.StartTime <= appointmentTime
        && appointmentTime.Add(appointmentTimeSpan) <= da.EndTime
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: Failed to Check doctor availability");
      throw;
    }
  }

  public async Task<
    ServiceResponse<Dictionary<Days, List<TimeRange>>>
  > GetDoctorAvailabilitiesAsync(Guid doctorId)
  {
    try
    {
      if (!await doctorService.CheckDoctorExistsAsync(doctorId))
      {
        return new ServiceResponse<Dictionary<Days, List<TimeRange>>>(
          false,
          404,
          null,
          "Doctor not found"
        );
      }

      var dayTimesMap = new Dictionary<Days, List<TimeRange>>();

      // Get doctor available days with start time and end time
      /*
        {
          Monday = [10,17]
          Tuesday = [12,16]
          Wednesday = [14,16]
        }
      */

      await appContext
        .DoctorAvailabilities.Where(da => da.DoctorId == doctorId)
        .ForEachAsync(da =>
        {
          if (!dayTimesMap.ContainsKey(da.AvailableDay))
            dayTimesMap[da.AvailableDay] = [];

          dayTimesMap[da.AvailableDay].Add(new TimeRange(da.StartTime, da.EndTime));
        });

      // Get doctor occupied appointment times for each days
      /*
      10:00 - 15:00 , 15:00 13:00 15:30
          AppointmentTimes = {
            Monday = [[10:00,17:00],[15:00,15:30],[15:30,16:00]]
          }
      */
      Console.WriteLine($"This is doctor available days: \n\n {dayTimesMap.Count} \n\n");

      var docAppTimes = await schedulingService.GetDoctorAppointmentTimesAsync(doctorId);

      Console.WriteLine($"This is doctor appointment times: \n\n {docAppTimes.Count} \n\n");

      // Write an algorithm that gives available remaining times for each day given the above inputs
      /*
        {
          Monday = [ [10:00,10:30],[10:30,11:00],..., [15:00,15:30],[15:30,16:00],[16:00,16:30],[16:30,17:00] ],
          ...
        }
      */

      var result = new Dictionary<Days, List<TimeRange>>();

      foreach (var (day, timeRangeAvail) in dayTimesMap) // O(dayTimesMap.Count)
      {
        foreach (TimeRange timeAvail in timeRangeAvail) // O(timeRangeAvail.Count)
        {
          TimeOnly startTime = timeAvail.StartTime;

          if (!docAppTimes.ContainsKey(day))
            continue;

          docAppTimes[day].Sort((a, b) => a.StartTime.CompareTo(b.StartTime)); // Sort the Time Array by the startTime in Ascending order

          for (int dayIndex = 0; dayIndex < docAppTimes[day].Count; dayIndex++) // O(docAppTimes.Count)
          {
            TimeOnly startTimeUnavail = docAppTimes[day][dayIndex].StartTime;
            TimeOnly endTimeUnavail = docAppTimes[day][dayIndex].EndTime;

            if (!result.ContainsKey(day))
              result[day] = [];

            if (startTime != startTimeUnavail)
              result[day].Add(new TimeRange(startTime, startTimeUnavail));

            startTime = endTimeUnavail;

            if (dayIndex == docAppTimes[day].Count - 1 && startTime != timeAvail.EndTime) { }
          }
        }
      }

      return new ServiceResponse<Dictionary<Days, List<TimeRange>>>
      {
        Data = result,
        Message = "Fetched doctor availabilities",
        StatusCode = 200,
        Success = true
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex} : An error occured trying to get doctor availabilities");
      throw;
    }
  }
}
