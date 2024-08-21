using System.Text.RegularExpressions;

namespace HealthHub.Source.Helpers.Extensions;

public static class StringExtensions
{
  public static string RemoveNonNumeric(this string phone)
  {
    return Regex.Replace(phone, @"[^0-9]+", "");
  }

  public static Guid ToGuid(this string str)
  {
    if (Guid.TryParse(str, out Guid result))
    {
      return result;
    }
    throw new ArgumentException($"'{str}' is not a valid GUID.");
  }

  public static T ConvertTo<T>(this string value)
  {
    try
    {
      return (T)Convert.ChangeType(value, typeof(T));
    }
    catch (Exception ex)
    {
      throw new InvalidCastException($"Cannot convert '{value}' to type '{typeof(T)}'.", ex);
    }
  }

  public static T ConvertToEnum<T>(this string value, bool ignoreCase = false)
    where T : struct, Enum
  {
    if (Enum.TryParse(value, ignoreCase, out T result))
    {
      return result;
    }
    throw new ArgumentException($"'{value}' is not a valid value for enum type '{typeof(T)}'.");
  }
}
