
using System.Text.RegularExpressions;

namespace HealthHub.Source.Extensions;

public static class StringExtensions
{

  public static string RemoveNonNumeric(this string phone)
  {
    return Regex.Replace(phone, @"[^0-9]+", "");
  }
}