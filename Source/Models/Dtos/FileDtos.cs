// Recieved from client to create a file

using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Defaults;

public record CreateFileDto(
  [Required] string MimeType,
  [Required] string FileDataBase64,
  [Required] string? FileName
);

// Recieved form client to edit a file
public record EditFileDto(Guid FileId, string? MimeType, string? FileDataBase64, string? FileName);

// Return to the Client
public record FileDto(Guid FileId, MimeDefaults MimeType, string FileDataBase64, string FileName);
