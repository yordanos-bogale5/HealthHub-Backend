namespace HealthHub.Source.Helpers;

public static class FileHelper
{
  public static async Task<string> ReadFile(string filePath)
  {
    if (!File.Exists(filePath))
    {
      throw new FileNotFoundException("The file was not found", filePath);
    }

    return await File.ReadAllTextAsync(filePath);
  }

  public static byte[] ToByteStream(string base64) => Convert.FromBase64String(base64);

  public static string ToBase64(byte[] byteStream) => Convert.ToBase64String(byteStream);
}
