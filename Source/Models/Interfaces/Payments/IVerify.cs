namespace HealthHub.Source.Models.Interfaces.Payments;

public interface IVerifyRequest
{
  string TransactionReference { get; set; }
}

public class VerifyRequest : IVerifyRequest
{
  public required string TransactionReference { get; set; }
}

public interface IVerifyResponse
{
  bool Success { get; set; }
  string FirstName { get; set; }
  string LastName { get; set; }
  string Email { get; set; }
}

public class VerifyResponse : IVerifyResponse
{
  public bool Success { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
}
