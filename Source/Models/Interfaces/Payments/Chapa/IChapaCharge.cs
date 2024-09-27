using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces.Payments;

namespace HealthHub.Source.Models.Interfaces.Payments.Chapa;

public class ChapaCharge : Charge
{
  public override required PaymentProvider PaymentProvider { get; init; } = PaymentProvider.Chapa;

  public required ChapaPaymentMethod PaymentMethod { get; set; }
}
