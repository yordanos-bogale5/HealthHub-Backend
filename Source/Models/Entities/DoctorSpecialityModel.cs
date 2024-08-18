using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Entities;

/// <summary>
/// Will use this for many-to-many relationship between doctors and specialities
/// </summary>
public class DoctorSpeciality
{
    public required Guid DoctorId { get; set; } // fk
    public virtual required Doctor Doctor { get; set; } // nav

    public required Guid SpecialityId { get; set; } // fk
    public virtual required Speciality Speciality { get; set; } // nav
}
