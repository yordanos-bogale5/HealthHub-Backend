using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public record CreateAppointmentDto
{
    [Required]
    public Guid DoctorId { get; init; }

    [Required]
    public Guid PatientId { get; init; }

    [Required]
    [DataType(DataType.Date)]
    [NotPastDate]
    public DateTime AppointmentDate { get; init; }

    [Required]
    public TimeOnly AppointmentTime { get; init; }

    [Required]
    public AppointmentType AppointmentType { get; init; }
}

public record AppointmentDto
{
    public required PatientDto Patient { get; init; }
    public required DoctorDto Doctor { get; init; }
    public DateTime AppointmentDate { get; init; }
    public TimeOnly AppointmentTime { get; init; }
    public AppointmentType AppointmentType { get; init; }
}
