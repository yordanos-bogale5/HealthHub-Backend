using HealthHub.Source.Models.Interfaces.Payments.Chapa;

public static class PaymentMethodExtensions
{
  public static Dictionary<ChapaPaymentMethod, string> ChapaPaymentMethods =
    new()
    {
      { ChapaPaymentMethod.TeleBirr, "telebirr" },
      { ChapaPaymentMethod.CbeBirr, "cbebirr" },
      // { ChapaPaymentMethod.Mpesa, "mpesa" },
      // { ChapaPaymentMethod.AwashBirr, "awash_birr" },
      // { ChapaPaymentMethod.Ebirr, "ebirr" },
      // { ChapaPaymentMethod.Amole, "amole" }
    };

  public static Dictionary<string, ChapaPaymentMethod> ChapaPaymentMethodsReverse =
    ChapaPaymentMethods.ToDictionary(x => x.Value, x => x.Key);

  public static string GetDisplayName(this ChapaPaymentMethod chapaPaymentMethod)
  {
    ChapaPaymentMethods.TryGetValue(chapaPaymentMethod, out var paymentMethod);
    return paymentMethod
      ?? throw new ArgumentException(
        $"Invalid payment method. Valid payment methods are {string.Join(", ", ChapaPaymentMethods.Values)}"
      );
  }

  public static bool IsValidChapaPaymentMethod(this string paymentMethod)
  {
    return ChapaPaymentMethods.ContainsValue(paymentMethod);
  }

  public static ChapaPaymentMethod ConvertToChapaPaymentMethod(this string value)
  {
    if (ChapaPaymentMethodsReverse.ContainsKey(value) == false)
      throw new ArgumentException(
        $"Invalid payment method. Valid payment methods are {string.Join(", ", ChapaPaymentMethods.Values)}"
      );
    return ChapaPaymentMethodsReverse[value];
  }
}
