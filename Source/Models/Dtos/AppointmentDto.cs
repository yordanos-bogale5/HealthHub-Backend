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
