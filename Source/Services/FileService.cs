namespace HealthHub.Source.Services;


public class FileService {
  public async Task<string> ReadFile(string filePath) {
    if (!File.Exists(filePath)) {
      throw new FileNotFoundException("The file was not found", filePath);
    }

    return await File.ReadAllTextAsync(filePath);
  }
}
