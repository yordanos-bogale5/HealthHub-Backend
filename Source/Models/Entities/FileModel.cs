using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Entities;

public class File
{
  public Guid FileId { get; set; } = Guid.NewGuid();
  public required string? FileName { get; set; }
  public required string MimeType { get; set; }

  [MaxLength(5242880)] // MaxSize = 5mb file
  public required byte[] FileData { get; set; } = [];
  public int FileSize => FileData.Length; // Derived from FileData
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
