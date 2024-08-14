using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Payment
{
  public Guid PaymentId { get; set; } = Guid.NewGuid();

  [Required]
  public required Guid DoctorId { get; set; } // <<FK>>
  [Required]
  public required Guid PatientId { get; set; } // <<FK>>

  [Required]
  public required decimal Amount { get; set; }
  [Required]
  public required PaymentStatus PaymentStatus { get; set; }
  [Required]
  public required PaymentProvider PaymentProvider { get; set; } = PaymentProvider.Chapa;
  public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

  public virtual Doctor? Doctor { get; set; } // <<NAV>>
  public virtual Patient? Patient { get; set; } // <<NAV>>
}