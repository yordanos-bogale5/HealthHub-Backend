using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Interfaces.Payments;

public interface ICharge
{
  string Amount { get; set; }

  PaymentCurrency Currency { get; set; }
  string PhoneNumber { get; set; }
  PaymentProvider PaymentProvider { get; init; }
}

public class Charge : ICharge
{
  public required string Amount { get; set; }
  public required PaymentCurrency Currency { get; set; }
  public required string PhoneNumber { get; set; }
  public virtual required PaymentProvider PaymentProvider { get; init; }
}

public interface IChargeRequest
{
  string Amount { get; set; }
  string Currency { get; set; }
  string PhoneNumber { get; set; }
  string PaymentProvider { get; set; }
  string PaymentMethod { get; set; }
}

public class ChargeRequest : IChargeRequest
{
  public required string Amount { get; set; }

  [Required]
  [PaymentCurrency]
  public required string Currency { get; set; }
  public required string PhoneNumber { get; set; }

  [Required]
  [PaymentProvider]
  public required string PaymentProvider { get; set; }

  public required string PaymentMethod { get; set; }
}

public interface IChargeResponse
{
  string RefId { get; set; }
  bool Status { get; set; }
}

public class ChargeResponse : IChargeResponse
{
  public required string Message { get; set; }
  public required string RefId { get; set; }
  public required bool Status { get; set; }
}
