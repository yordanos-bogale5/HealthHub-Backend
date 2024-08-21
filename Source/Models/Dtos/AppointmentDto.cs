using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

/// <summary>
/// This is what the client sends to create an appointment
/// </summary>
public record CreateAppointmentDto
{
  public required string DoctorId { get; init; }
  public required string PatientId { get; init; }
  public required string AppointmentDate { get; init; }
  public required string AppointmentTime { get; init; }
  public required string AppointmentType { get; init; }

  public void Deconstruct(
    out string doctorId,
    out string patientId,
    out string appointmentDate,
    out string appointmentTime,
    out string appointmentType
  )
  {
    doctorId = DoctorId;
    patientId = PatientId;
    appointmentDate = AppointmentDate;
    appointmentTime = AppointmentTime;
    appointmentType = AppointmentType;
  }
}

public record AppointmentDto
{
  public required PatientDto Patient { get; init; }
  public required DoctorDto Doctor { get; init; }
  public DateTime AppointmentDate { get; init; }
  public TimeOnly AppointmentTime { get; init; }
  public AppointmentType AppointmentType { get; init; }
}
