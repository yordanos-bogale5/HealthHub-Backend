using HealthHub.Migrations;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using HealthHub.Source.Models.Responses;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace HealthHub.Source.Services;

public class AppointmentService(
  ApplicationContext appContext,
  DoctorService doctorService,
  PatientService patientService,
  ILogger<AppointmentService> logger
)
{
  /// <summary>
  /// Creates an appointment given createAppointmentDto payload
  /// </summary>
  /// <param name="createAppointmentDto"></param>
  /// <returns>The newly created appointment as dto</returns>
  public async Task<ServiceResponse<AppointmentDto>> CreateAppointmentAsync(
    CreateAppointmentDto createAppointmentDto
  )
  {
    try
    {
      Guid doctorId = createAppointmentDto.DoctorId.ToGuid();
      Guid patientId = createAppointmentDto.PatientId.ToGuid();

      var doctor = await doctorService.GetDoctorAsync(doctorId);
      if (!doctor.Success || doctor.Data == null)
      {
        return new ServiceResponse<AppointmentDto>
        {
          StatusCode = doctor.StatusCode,
          Success = doctor.Success,
          Data = null,
          Message = doctor.Message
        };
      }

      var patient = await patientService.GetPatientAsync(patientId);

      if (!patient.Success || patient.Data == null)
      {
        return new ServiceResponse<AppointmentDto>
        {
          StatusCode = patient.StatusCode,
          Success = patient.Success,
          Data = null,
          Message = patient.Message
        };
      }

      // Destructuring and parsing
      DateOnly appointmentDate = createAppointmentDto.AppointmentDate.ConvertTo<DateOnly>();
      TimeOnly appointmentTime = TimeOnly.Parse(createAppointmentDto.AppointmentTime);

      AppointmentType appointmentType =
        createAppointmentDto.AppointmentType.ConvertToEnum<AppointmentType>();

      DayOfWeek appointmentDay = appointmentDate
        .DayOfWeek.GetDisplayName()
        .ConvertToEnum<DayOfWeek>();

      // Create the appointment
      Appointment appointmentData = new Appointment
      {
        DoctorId = doctorId,
        PatientId = patientId,
        AppointmentDate = appointmentDate,
        AppointmentTime = appointmentTime,
        AppointmentType = appointmentType,
        Doctor = doctor.Data,
        Patient = patient.Data
      };

      TimeSpan appointmentTimeSpan = TimeSpan.TryParse(
        createAppointmentDto.AppointmentTimeSpan,
        out var timeSpan
      )
        ? timeSpan
        : appointmentData.AppointmentTimeSpan;

      // Check if the doctor is free at that day and time (Check Doctor Availability Table)
      bool isDoctorAvail = await doctorService.CheckDoctorAvailabilityAsync(
        doctorId,
        appointmentDay,
        appointmentTime,
        appointmentTimeSpan
      );

      if (!isDoctorAvail)
      {
        return new ServiceResponse<AppointmentDto>
        {
          StatusCode = 200,
          Success = false,
          Data = null,
          Message = "Doctor is not available at that day and time.",
        };
      }

      // Check if other patients are scheduled for that day and time (Check Appointment Table)
      isDoctorAvail = await CheckAppointmentAvailabilityAsync(
        doctorId,
        appointmentDate,
        appointmentTime
      );

      if (!isDoctorAvail)
      {
        return new ServiceResponse<AppointmentDto>
        {
          StatusCode = 200,
          Success = false,
          Data = null,
          Message = "Doctor has an appointment at that day and time.",
        };
      }

      var appointment = await appContext.Appointments.AddAsync(appointmentData);

      await appContext.SaveChangesAsync();

      return new ServiceResponse<AppointmentDto>
      {
        StatusCode = 201,
        Success = true,
        Data = appointment.Entity.ToAppointmentDto(
          doctor.Data,
          patient.Data,
          doctor.Data.User,
          patient.Data.User,
          doctor.Data.DoctorSpecialities
        ),
        Message = "Appointment created successfully",
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to create an appointment");
      throw;
    }
  }

  /// <summary>
  /// Checks if an appointment slot is available for the specified doctor on the given date and time.
  /// </summary>
  /// <param name="doctorId">The ID of the doctor whose appointment availability is being checked.</param>
  /// <param name="newAppointmentDate"></param>
  /// <param name="newAppointmentStartTime"></param>
  /// <returns>True if the appointment slot is available (i.e. no appointment exists for that doctor, date, and time); otherwise, false.</returns>
  public async Task<bool> CheckAppointmentAvailabilityAsync(
    Guid doctorId,
    DateOnly newAppointmentDate,
    TimeOnly newAppointmentStartTime
  )
  {
    try
    {
      var result = await appContext.Appointments.ToListAsync();
      return !result.Any(app =>
        app.DoctorId == doctorId
        && app.AppointmentDate == newAppointmentDate
        && newAppointmentStartTime < app.AppointmentTime.Add(app.AppointmentTimeSpan)
        && newAppointmentStartTime.Add(app.AppointmentTimeSpan) > app.AppointmentTime
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to check appointment availability");
      throw;
    }
  }

  /// <summary>
  /// Retrieves all appointments from the database
  /// </summary>
  /// <returns>A list of appointment dtos</returns>
  public async Task<ServiceResponse<List<AppointmentDto>>> GetAllAppointmentsAsync()
  {
    try
    {
      var result = await appContext
        .Appointments.Include(a => a.Doctor) // include doctor
        .ThenInclude(d => d.User) // doctorUser
        .Include(a => a.Doctor.DoctorSpecialities) // include doctor specialties
        .Include(a => a.Patient) // include patietn
        .ThenInclude(p => p.User) // patient user
        .Select(a =>
          // Map each appointment and convert each entry to appointmentDto
          // with the above included nav models
          a.ToAppointmentDto(
            a.Doctor,
            a.Patient,
            a.Doctor.User,
            a.Patient.User,
            a.Doctor.DoctorSpecialities
          )
        )
        .ToListAsync();

      return new ServiceResponse<List<AppointmentDto>>(
        true,
        200,
        result,
        "Fetched all appointments."
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Failed to get all appointments: {ex} ");
      throw;
    }
  }

  /// <summary>
  /// Deletes the appointment
  /// </summary>
  /// <param name="appointmentId"></param>
  /// <returns>No content(204) if successful </returns>
  /// <exception cref="KeyNotFoundException"/>
  public async Task<ServiceResponse> DeleteAppointmentAsync(Guid appointmentId)
  {
    try
    {
      var appointment = await appContext.Appointments.FirstOrDefaultAsync(app =>
        app.AppointmentId == appointmentId
      );

      if (appointment == null)
      {
        throw new KeyNotFoundException("Appointment not found");
      }

      appContext.Remove(appointment);

      await appContext.SaveChangesAsync();

      return new ServiceResponse(true, 204, "Appointment deleted successfully");
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Error occured trying to delete appointment in service: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Deletes all appointments of the user specified by the id (userid)
  /// </summary>
  /// <param name="userId"></param>
  /// <returns>void</returns>
  public async Task DeleteAppointmentWhereUserIdAsync(Guid userId)
  {
    try
    {
      var appointments = await appContext
        .Appointments.Include(ap => ap.Doctor)
        .Include(ap => ap.Patient)
        .Where(ap => ap.Doctor.UserId == userId || ap.Patient.UserId == userId)
        .ToListAsync();

      if (appointments.Count == 0)
      {
        logger.LogInformation("No appointments found for user with the specified id");
        return;
      }

      appContext.RemoveRange(appointments);

      await appContext.SaveChangesAsync();
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Error occured trying to delete appointment in service: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Retrieves all patient appointments given by the patientId
  /// </summary>
  /// <param name="patientId"></param>
  /// <returns>A list of appointment dtos</returns>
  public async Task<ServiceResponse<List<AppointmentDto>>> GetPatientAppointmentsAsync(
    Guid patientId
  )
  {
    try
    {
      if (!await patientService.CheckPatientExistsAsync(patientId))
      {
        return new ServiceResponse<List<AppointmentDto>>(false, 404, null, "Patient not found");
      }

      var result = await appContext
        .Appointments.Where(ap => ap.PatientId == patientId)
        .Include(ap => ap.Doctor)
        .ThenInclude(d => d.User)
        .Include(ap => ap.Doctor.DoctorSpecialities)
        .Select(ap => ap.ToAppointmentDto(ap.Doctor, ap.Doctor.User, ap.Doctor.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<AppointmentDto>>(
        true,
        200,
        result,
        "Fetched all patient appointments."
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to get patient appointments {ex}");
      throw;
    }
  }

  /// <summary>
  /// Retrieves all appointments for the specified doctorId
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>A list of appointment dtos</returns>
  public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(Guid doctorId)
  {
    try
    {
      if (!await doctorService.CheckDoctorExistsAsync(doctorId))
      {
        throw new KeyNotFoundException("Doctor not found");
      }

      var result = await appContext
        .Appointments.Where(ap => ap.DoctorId == doctorId)
        .Include(ap => ap.Patient)
        .ThenInclude(p => p.User) // populate user
        .Select(ap => ap.ToAppointmentDto(ap.Patient, ap.Patient.User))
        .ToListAsync();

      return result;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to get doctor appointments {ex}");
      throw;
    }
  }

  /// <summary>
  /// Edits the appointment information for the specified appointmentId
  /// </summary>
  /// <param name="editAppointmentDto"></param>
  /// <param name="appointmentId"></param>
  /// <returns>The newly updated appointment</returns>
  public async Task<ServiceResponse<AppointmentDto>> EditAppointmentAsync(
    EditAppointmentDto editAppointmentDto,
    Guid appointmentId
  )
  {
    try
    {
      // Retrieve the appointment
      var appointment = await appContext
        .Appointments.Include(ap => ap.Patient) // include patient
        .ThenInclude(p => p.User) // include patientUser
        .Include(ap => ap.Doctor) // include doctor
        .ThenInclude(d => d.User) // include doctorUser
        .Include(ap => ap.Doctor.DoctorSpecialities) // include doctor specialities
        .FirstOrDefaultAsync(ap => ap.AppointmentId == appointmentId);

      // Check if the appointment exists
      if (appointment == null)
      {
        return new ServiceResponse<AppointmentDto>(
          false,
          404,
          null,
          "Appointment with the specified id not found"
        );
      }

      var doctorId = editAppointmentDto.DoctorId?.ToGuid();
      var appointmentDate = editAppointmentDto.AppointmentDate?.ConvertTo<DateOnly>();
      DayOfWeek? appointmentDay = appointmentDate
        ?.DayOfWeek.GetDisplayName()
        .ConvertToEnum<DayOfWeek>();
      var appointmentTime = TimeOnly.TryParse(editAppointmentDto.AppointmentTime, out var appTime)
        ? appTime
        : (TimeOnly?)null;
      var appointmentType = editAppointmentDto.AppointmentType?.ConvertTo<AppointmentType>();

      if (doctorId != null)
      {
        // Check if a doctor with the doctorId exists
        if (!await doctorService.CheckDoctorExistsAsync(doctorId.Value))
        {
          return new ServiceResponse<AppointmentDto>(
            false,
            404,
            null,
            "Doctor with the specified id not found"
          );
        }

        appointment.DoctorId = doctorId.Value;
      }

      bool isAppointmentAvailable = await CheckAppointmentAvailabilityAsync(
        doctorId ?? appointment.DoctorId,
        appointmentDate ?? appointment.AppointmentDate,
        appointmentTime ?? appointment.AppointmentTime
      );

      bool isDoctorAvailable = await doctorService.CheckDoctorAvailabilityAsync(
        doctorId ?? appointment.DoctorId,
        appointmentDay
          ?? appointment.AppointmentDate.DayOfWeek.GetDisplayName().ConvertToEnum<DayOfWeek>(),
        appointmentTime ?? appointment.AppointmentTime,
        appointment.AppointmentTimeSpan
      );

      if (!(isAppointmentAvailable && isDoctorAvailable))
      {
        return new ServiceResponse<AppointmentDto>(
          false,
          400,
          null,
          "Doctor is not available at that day or time."
        );
      }

      if (appointmentDate != null)
        appointment.AppointmentDate = appointmentDate.Value;

      if (appointmentTime != null)
        appointment.AppointmentTime = appointmentTime.Value;

      if (appointmentType != null)
        appointment.AppointmentType = appointmentType.Value;

      await appContext.SaveChangesAsync(); // Save the updates

      return new ServiceResponse<AppointmentDto>
      {
        StatusCode = 200,
        Success = true,
        Data = appointment.ToAppointmentDto(
          appointment.Doctor,
          appointment.Patient,
          appointment.Doctor.User,
          appointment.Patient.User,
          appointment.Doctor.DoctorSpecialities
        ),
        Message = "Appointment edited successfully"
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to edit appointment {ex}");
      throw;
    }
  }

  public async Task<bool> CheckAppointmentExistsAsync(Guid appointmentId)
  {
    try
    {
      return await appContext.Appointments.AnyAsync(ap => ap.AppointmentId == appointmentId);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to check if appointment exists {ex}");
      throw;
    }
  }

  /// <summary>
  /// Gets the doctor appointment schedules
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns><see cref="DoctorSchedules"/> if the doctor has appointments, otherwise null.</returns>
  public DoctorSchedules? GetDoctorAppointmentSchedules(Guid doctorId)
  {
    try
    {
      Dictionary<DateOnly, List<DoctorSchedule>> result = [];

      var appointments = appContext.Appointments.Where(a =>
        a.DoctorId == doctorId && a.AppointmentDate >= DateOnly.FromDateTime(DateTime.Today)
      );

      if (!appointments.Any())
        return null;

      foreach (Appointment appointment in appointments)
      {
        if (result.ContainsKey(appointment.AppointmentDate))
        {
          result[appointment.AppointmentDate]
            .Add(
              new DoctorSchedule(
                new TimeRange(
                  appointment.AppointmentTime,
                  appointment.AppointmentTime.Add(appointment.AppointmentTimeSpan)
                ),
                false // not free because he/she has an appointment
              )
            );
        }
        else
        {
          result[appointment.AppointmentDate] =
          [
            new DoctorSchedule(
              new TimeRange(
                appointment.AppointmentTime,
                appointment.AppointmentTime.Add(appointment.AppointmentTimeSpan)
              ),
              false
            )
          ];
        }
      }

      return new DoctorSchedules(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to get doctor appointment schedules.");
      throw;
    }
  }

  /// <summary>
  /// Gets all the schedules of the doctor within the given time frame
  /// </summary>
  /// <param name="doctorId"></param>
  /// <param name="timeFrame"></param>
  /// <returns></returns>
  public async Task<DoctorSchedules?> GetDoctorSchedulesAsync(Guid doctorId, TimeFrame timeFrame)
  {
    try
    {
      // Get doctor available DayOfWeek with the time ranges they are available in
      Dictionary<DayOfWeek, List<TimeRange>> doctorAvailabilities =
        await doctorService.GetDoctorAvailabilitiesAsync(doctorId);

      // Sort the list of time ranges in each day of doctor availabilities in ascending order of startTime
      foreach (var da in doctorAvailabilities)
        da.Value.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

      // Get doctor appointment schedules
      var doctorAppointmentSchedulesResponse = GetDoctorAppointmentSchedules(doctorId);

      Dictionary<DateOnly, List<DoctorSchedule>> doctorAppointments =
        doctorAppointmentSchedulesResponse == null ? [] : doctorAppointmentSchedulesResponse.Data;

      // Sort likewise  the doctor appointment timeranges
      foreach (var da in doctorAppointments)
        da.Value.Sort((a, b) => a.TimeRange.StartTime.CompareTo(b.TimeRange.StartTime));

      // This is what we'll return (a date to schedules map)
      var result = new Dictionary<DateOnly, List<DoctorSchedule>>();

      var timeFrameLength = (int)timeFrame;

      DateOnly curDate = DateOnly.FromDateTime(DateTime.Today);
      for (int i = 0; i < timeFrameLength; i++)
      {
        if (doctorAvailabilities.ContainsKey(curDate.DayOfWeek))
          result[curDate] = doctorAvailabilities[curDate.DayOfWeek]
            .Select(timeRange => new DoctorSchedule(timeRange, true))
            .ToList();
        curDate = curDate.AddDays(1);
      }

      // TODO: Finish  the algorithm
      // Currenlty it stops by giving default value [] for the given timeFrame
      // Your task is to write an algorithm that will populate each date with the doctor avaiable times for that date
      // considering also the appointments that are present on that day (marking free/notFree each doctorSchedule)

      foreach (DateOnly appDate in doctorAppointments.Keys)
      {
        foreach (DayOfWeek day in doctorAvailabilities.Keys)
        {
          if (appDate.DayOfWeek != day)
            continue;

          result[appDate] = []; // Empty the result appDate entry because we will populate it dynamically from here forth

          List<DoctorSchedule> appointmentSchedule = doctorAppointments[appDate];
          List<TimeRange> availabilityScheduleRanges = doctorAvailabilities[day];

          TimeOnly startTime = availabilityScheduleRanges[0].StartTime;
          TimeOnly endTime = availabilityScheduleRanges[0].EndTime;

          for (var j = 0; j < appointmentSchedule.Count; j++)
          {
            if (!result.ContainsKey(appDate))
              continue;

            var (timeRange, isFree) = appointmentSchedule[j];

            if (startTime != timeRange.StartTime)
              result[appDate]
                .Add(new DoctorSchedule(new TimeRange(startTime, timeRange.StartTime), true));

            result[appDate].Add(new DoctorSchedule(timeRange, isFree));

            startTime = timeRange.EndTime;

            if (j == appointmentSchedule.Count - 1 && startTime != endTime)
              result[appDate].Add(new DoctorSchedule(new TimeRange(startTime, endTime), true));
          }
        }
      }

      return new DoctorSchedules(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to get doctor schedules in service");
      throw;
    }
  }
}
