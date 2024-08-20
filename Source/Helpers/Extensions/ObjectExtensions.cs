using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HealthHub.Source.Helpers.Extensions;

public static class ObjectExtensions
{
  public static bool IsNull(this object? obj) => obj is null;

  public static bool IsTypeOf<T>(this object? obj) => obj is T;

  // public static T ConvertTo<T>(this object obj) => (T)Convert.ChangeType(obj, typeof(T));
}
