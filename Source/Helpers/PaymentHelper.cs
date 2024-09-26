public static class PaymentHelper
{
  public static string GetTransactionReference()
  {
    return "tx" + Guid.NewGuid().ToString();
  }
}
