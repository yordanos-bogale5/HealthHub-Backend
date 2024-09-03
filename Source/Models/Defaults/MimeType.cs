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
  private static Dictionary<MimeDefaults, string> Mimes =
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

  public static string GetMime(MimeDefaults mimeDefault)
  {
    return Mimes[mimeDefault];
  }
}
