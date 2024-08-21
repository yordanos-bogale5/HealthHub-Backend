using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace HealthHub.Source.Services;

public class AppointmentService(
  ApplicationContext appContext,
  DoctorService doctorService,
  PatientService patientService,
  AvailabilityService availabilityService,
  ILogger<AppointmentService> logger
)
{
  public async Task<ServiceResponse<AppointmentDto>> CreateAppointmentAsync(
    CreateAppointmentDto createAppointmentDto,
    int appointmentTimePeriod = 30 // Change if business logic changes
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

      DateTime appointmentDate = createAppointmentDto.AppointmentDate.ConvertTo<DateTime>();
      TimeOnly appointmentTime = TimeOnly.Parse(createAppointmentDto.AppointmentTime);

      AppointmentType appointmentType =
        createAppointmentDto.AppointmentType.ConvertToEnum<AppointmentType>();

      Days appointmentDay = appointmentDate.DayOfWeek.GetDisplayName().ConvertToEnum<Days>();

      // This is the time period for each appointment
      TimeSpan appointmentTimeSpan = TimeSpan.FromMinutes(appointmentTimePeriod);

      // Check if the doctor is free at that day and time (Check Doctor Availability Table)
      bool isDoctorAvail = await availabilityService.CheckDoctorAvailabilityAsync(
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
          Message = "Doctor has an appointment at that day and time.",
        };
      }

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

      var appointment = await appContext.Appointments.AddAsync(appointmentData);

      await appContext.SaveChangesAsync();

      return new ServiceResponse<AppointmentDto>
      {
        StatusCode = 201,
        Success = true,
        Data = appointment.Entity.ToAppointmentDto(doctor.Data, patient.Data),
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
  /// <param name="appointmentTimeSpan"></param>
  /// <returns>True if the appointment slot is available (i.e., no appointment exists for that doctor, date, and time); otherwise, false.</returns>
  public async Task<bool> CheckAppointmentAvailabilityAsync(
    Guid doctorId,
    DateTime newAppointmentDate,
    TimeOnly newAppointmentStartTime,
    TimeSpan appointmentTimeSpan
  )
  {
    try
    {
      TimeOnly newAppointmentEndTime = newAppointmentStartTime.Add(appointmentTimeSpan);
      var result = await appContext.Appointments.ToListAsync();
      return !result.Any(app =>
        app.DoctorId == doctorId
        && app.AppointmentDate == newAppointmentDate
        && newAppointmentStartTime < app.AppointmentTime.Add(appointmentTimeSpan)
        && newAppointmentEndTime > app.AppointmentTime
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to check appointment availability");
      throw;
    }
  }

  public async Task<ServiceResponse<List<AppointmentDto>>> GetAllAppointmentsAsync()
  {
    try
    {
      var result = await appContext
        .Appointments.Include(a => a.Doctor)
        .ThenInclude(d => d.User)
        .Include(a => a.Patient)
        .ThenInclude(p => p.User)
        .Select(a => a.ToAppointmentDto(a.Doctor, a.Patient))
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

  public async Task<ServiceResponse<List<AppointmentDto>>> GetPatientAppointmentsAsync(
    Guid patientId
  )
  {
    try
    {
      if (!await doctorService.CheckDoctorExistsAsync(patientId))
      {
        return new ServiceResponse<List<AppointmentDto>>(false, 404, null, "Patient not found");
      }

      var result = await appContext
        .Appointments.Where(ap => ap.PatientId == patientId)
        .Include(ap => ap.Doctor)
        .ThenInclude(d => d.User)
        .Select(ap => ap.ToAppointmentDto(ap.Doctor, null))
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

  public async Task<ServiceResponse<List<AppointmentDto>>> GetDoctorAppointmentsAsync(Guid doctorId)
  {
    try
    {
      if (!await doctorService.CheckDoctorExistsAsync(doctorId))
      {
        return new ServiceResponse<List<AppointmentDto>>(false, 404, null, "Doctor not found");
      }

      var result = await appContext
        .Appointments.Where(ap => ap.DoctorId == doctorId)
        .Include(ap => ap.Patient)
        .ThenInclude(p => p.User)
        .Select(ap => ap.ToAppointmentDto(null, ap.Patient))
        .ToListAsync();

      return new ServiceResponse<List<AppointmentDto>>(
        true,
        200,
        result,
        "Fetched all doctor appointments."
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to get doctor appointments {ex}");
      throw;
    }
  }
}
