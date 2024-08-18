using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Models.Dtos;

public record CreateDoctorSpecialityDto
{
    public required Doctor Doctor;
    public required Speciality Speciality;
}
