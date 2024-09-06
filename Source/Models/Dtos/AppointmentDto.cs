using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Entities;
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
  public string? AppointmentTimeSpan { get; init; }
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

/// <summary>
/// This is what the client sends to edit an appointment
/// </summary>
public record EditAppointmentDto
{
  public string? DoctorId { get; init; }
  public string? AppointmentDate { get; init; }
  public string? AppointmentTime { get; init; }
  public string? AppointmentType { get; init; }

  public void Deconstruct(
    out string? doctorId,
    out string? appointmentDate,
    out string? appointmentTime,
    out string? appointmentType
  )
  {
    doctorId = DoctorId;
    appointmentDate = AppointmentDate;
    appointmentTime = AppointmentTime;
    appointmentType = AppointmentType;
  }
}

public record AppointmentDto
{
  public required Guid AppointmentId { get; init; }
  public PatientDto? Patient { get; init; }
  public DoctorDto? Doctor { get; init; }
  public required DateOnly AppointmentDate { get; init; }
  public required TimeOnly AppointmentTime { get; init; }
  public required AppointmentType AppointmentType { get; init; }
}
