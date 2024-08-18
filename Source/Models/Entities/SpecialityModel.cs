using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class Speciality
{
    public Guid SpecialityId { get; set; }

    [Required]
    public required string SpecialityName { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<DoctorSpeciality> DoctorSpecialities { get; set; } =
        new HashSet<DoctorSpeciality>();
}
