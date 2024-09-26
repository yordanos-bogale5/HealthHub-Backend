using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class DoctorPreference
{
  [Key]
  public required Guid DoctorId { get; set; }
  public virtual Doctor? Doctor { get; set; }

  // [Range(0, 1)] // Default is 0 being 0% discount, 1 being 100% discount (free)
  // public decimal Discount { get; set; } = 0;

  // public TimeSpan DiscountTimeSpan { get; set; } = TimeSpan.Zero;
  // public DateTime? DiscountEndDate { get; set; }

  public required decimal OnlineAppointmentFee { get; set; }
  public required decimal InPersonAppointmentFee { get; set; }
}
