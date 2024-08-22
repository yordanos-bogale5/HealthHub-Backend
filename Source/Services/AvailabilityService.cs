using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;

public class AvailabilityService(ApplicationContext appContext, ILogger<AvailabilityService> logger)
{
  /// <summary>
  /// Creates doctor availability entries given a List of tuple having (Day, StartTime, EndTime)
  /// </summary>
  /// <param name="doctorAvailabilities"></param>
  /// <param name="doctor"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<List<DoctorAvailability>>> AddDoctorAvailabilityAsync(
    List<Availability> doctorAvailabilities,
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
}
