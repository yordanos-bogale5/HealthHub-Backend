namespace HealthHub.Source.Models.Entities;

public class File {
  public Guid FileId { get; set; } = Guid.NewGuid();
  public required string MimeType { get; set; }
  public required byte[] FileData { get; set; }
}
