using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class DoctorService(
  ApplicationContext appContext,
  SpecialityService specialityService,
  DoctorSpecialityService doctorSpecialityService,
  ILogger<DoctorService> logger
)
{
  public async Task<Doctor?> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
  {
    try
    {
      var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
      var doctor = doctorResult.Entity;
      logger.LogInformation($"\nCreate Doctor Result: \n {doctorResult}");
      await appContext.SaveChangesAsync();
      return doctor;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Doctor");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetAllDoctors()
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(true, 200, doctorUsers, "All Doctors Retrieved!");
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get all Doctors in User Service: {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsBySpecialityAsync(
    string specialityName
  )
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(ds.Speciality.SpecialityName, $"%{specialityName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with speciality name retrieved"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get doctors by speciality in doctor service {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByNameAsync(string doctorName)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(d.User.FirstName + " " + d.User.LastName, $"%{doctorName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with name retrieved"
      );
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByGenderAsync(Gender gender)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d => d.User.Gender == gender)
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with gender retrieved"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Getting doctors by gender failed: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Gets a doctor with doctorId and returns it with User, DoctorSpecialities and Speciality fields Populated
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>Populated Doctor Model</returns>
  public async Task<ServiceResponse<Doctor>> GetDoctorAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .SingleOrDefaultAsync(d => d.DoctorId == doctorId);

      if (doctor == null)
      {
        return new ServiceResponse<Doctor>(false, 404, null, "Doctor not found");
      }

      return new ServiceResponse<Doctor>(true, 200, doctor, "Doctor found");
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get doctor by id {ex}");
      throw;
    }
  }

  /// <summary>
  /// Checks if a doctor exists
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>True if doctor exists, otherwise False </returns>
  public async Task<bool> CheckDoctorExistsAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext.Doctors.FindAsync(doctorId);
      return doctor != null;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to check if doctor exists {ex}");
      throw;
    }
  }

  public async Task<DoctorProfileDto> EditDoctorProfileAsync(
    EditDoctorProfileDto editDoctorProfileDto
  )
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Include(d => d.DoctorAvailabilities)
        .FirstOrDefaultAsync(d => d.UserId == editDoctorProfileDto.UserId);

      if (doctor == null)
      {
        throw new BadHttpRequestException("Doctor with that userId is not present.");
      }

      /* Creating the New Specialtities */
      var specialities =
        editDoctorProfileDto.Specialitites != null
          ? await specialityService.CreateSpecialitiesAsync(
            editDoctorProfileDto.Specialitites.ToSpecialityList(doctor.DoctorId)
          )
          : null;

      // Create doctor specialities based on the new specialities
      var docSpecs =
        specialities != null
          ? await doctorSpecialityService.CreateDoctorSpecialitiesAsync(
            specialities.Select(s => s.ToCreateDoctorSpecialityDto(doctor)).ToList()
          )
          : null;

      /* Creating the New Doctor Availabilities */
      var availabilitiesResponse =
        editDoctorProfileDto.Availabilities != null
          ? await AddDoctorAvailabilityAsync(editDoctorProfileDto.Availabilities, doctor)
          : null;

      /* Perform Updates */
      doctor.DoctorSpecialities = docSpecs ?? doctor.DoctorSpecialities;

      doctor.Qualifications = editDoctorProfileDto.Qualifications ?? doctor.Qualifications;

      doctor.DoctorStatus =
        editDoctorProfileDto.DoctorStatus != null
          ? editDoctorProfileDto.DoctorStatus.ConvertToEnum<DoctorStatus>()
          : doctor.DoctorStatus;

      doctor.Biography = editDoctorProfileDto.Biography ?? doctor.Biography;

      doctor.DoctorAvailabilities =
        availabilitiesResponse != null
          ? availabilitiesResponse.Data ?? doctor.DoctorAvailabilities
          : doctor.DoctorAvailabilities;

      await appContext.SaveChangesAsync();

      return doctor.ToDoctorProfileDto(
        doctor.User,
        doctor.DoctorAvailabilities,
        doctor.DoctorSpecialities.Where(s => s != null).Select(ds => ds.Speciality!).ToList()
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to edit doctor.");
      throw;
    }
  }

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
      if (!await CheckDoctorExistsAsync(doctorId))
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

      var docAppTimes = await GetDoctorAppointmentTimesAsync(doctorId);

      if (docAppTimes.Count == 0)
      {
        return new ServiceResponse<Dictionary<Days, List<TimeRange>>>(
          true,
          200,
          null,
          "The Doctor doesn't have any appointments"
        );
      }

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
          {
            result[day] = timeRangeAvail;
            continue;
          }

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

            if (dayIndex == docAppTimes[day].Count - 1 && startTime != timeAvail.EndTime)
            {
              result[day].Add(new TimeRange(startTime, timeAvail.EndTime));
            }
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

  /// <summary>
  /// Retrieves the profile of the doctor specified with the userId
  /// </summary>
  /// <param name="userId"></param>
  /// <returns>A <see cref="DoctorProfileDto"/> representing the doctor's profile.</returns>
  /// <exception cref="KeyNotFoundException"/>Thrown when no doctor is found with the specified userId.<exception/>
  public async Task<DoctorProfileDto> GetDoctorProfileAsync(Guid userId)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Where(d => d.UserId == userId)
        .Include(d => d.User)
        .Include(d => d.DoctorAvailabilities)
        .Include(d => d.DoctorSpecialities)
        .SingleOrDefaultAsync();

      if (doctor == null)
      {
        throw new KeyNotFoundException(
          "Doctor with that user id is not found. Couldn't retrieve profile information."
        );
      }

      return doctor.ToDoctorProfileDto(
        doctor.User,
        doctor.DoctorAvailabilities,
        doctor
          .DoctorSpecialities.Where(ds => ds.Speciality != null)
          .Select(ds => ds.Speciality!)
          .ToList()
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured tring to get doctor profile");
      throw;
    }
  }
}
