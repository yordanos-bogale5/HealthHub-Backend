using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Views;

public record MessageDto(Guid SenderId, Guid ReceiverId, string? MessageText, List<FileDto>? Files);

public record CreateMessageDto(
  [Guid] Guid SenderId,
  [Guid] Guid ReceiverId,
  [MinLength(1)] string? MessageText,
  [ValidCreateFileList] List<CreateFileDto>? Files
);
