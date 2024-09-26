using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Views;

public record MessageDto(Guid MessageId, Guid? SenderId, string? MessageText, List<FileDto>? Files);

public record CreateMessageDto(
  [Guid] Guid ConversationId,
  [Guid] Guid SenderId,
  [MinLength(1)] string? MessageText,
  [ValidCreateFileList] List<CreateFileDto>? Files
);
