namespace HealthHub.Source.Models.Defaults;

public enum MimeDefaults
{
  Jpeg,
  Png,
  Gif,
  Bmp,
  Webp,
  Tiff,
  Mp4,
  Webm,
  Avi,
  Mpeg,
  Mkv,
  Mp3,
  Wav,
  Ogg,
}

public static class Mime
{
  public static readonly Dictionary<MimeDefaults, string> Mimes =
    new()
    {
      { MimeDefaults.Jpeg, "image/jpeg" },
      { MimeDefaults.Png, "image/png" },
      { MimeDefaults.Gif, "image/gif" },
      { MimeDefaults.Bmp, "image/bmp" },
      { MimeDefaults.Webp, "image/webp" },
      { MimeDefaults.Tiff, "image/tiff" },
      { MimeDefaults.Mp4, "video/mp4" },
      { MimeDefaults.Webm, "video/webm" },
      { MimeDefaults.Avi, "video/x-msvideo" },
      { MimeDefaults.Mpeg, "video/mpeg" },
      { MimeDefaults.Mkv, "video/x-matroska" },
      { MimeDefaults.Mp3, "audio/mp3" },
      { MimeDefaults.Wav, "audio/wav" },
      { MimeDefaults.Ogg, "audio/ogg" },
    };

  // For performing reverse lookup efficiently
  public static readonly Dictionary<string, MimeDefaults> ReverseMimes = Mimes.ToDictionary(
    kvp => kvp.Value,
    kvp => kvp.Key
  );

  public static string GetMime(MimeDefaults mimeDefault)
  {
    return Mimes[mimeDefault];
  }

  public static string? GetMime(string mimeDefault)
  {
    if (!Enum.TryParse(mimeDefault, out MimeDefaults md))
      return null;
    return Mimes[md];
  }

  public static List<MimeDefaults> GetImageMimes()
  {
    return Mimes.Where(kvp => kvp.Value.StartsWith("image")).Select(kvp => kvp.Key).ToList();
  }

  /// <summary>
  /// Returns true if the mime value is supported otherwise false.
  /// </summary>
  /// <param name="mimeValue"></param>
  /// <returns>
  /// image/jpeg -> return true
  /// image/j -> retuns false
  /// </returns>
  public static bool IsSupportedMimeValue(string mimeValue)
  {
    return ReverseMimes.ContainsKey(mimeValue);
  }

  public static MimeDefaults GetReverseMime(string mimeValue)
  {
    return ReverseMimes[mimeValue];
  }
}
