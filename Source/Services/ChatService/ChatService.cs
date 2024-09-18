using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace HealthHub.Source.Services.ChatService;

public class ChatService(
  ApplicationContext appContext,
  UserService userService,
  FileService fileService,
  ILogger<ChatService> logger
) : IChatService
{
  public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId)
  {
    try
    {
      var conversation = await appContext
        .Conversations.Where(c => c.ConversationId == conversationId)
        .Include(c => c.Messages)
        .ThenInclude(m => m.Files) // Load the associated files
        .Include(c => c.Messages)
        .ThenInclude(m => m.Sender) // Load the sender
        .Include(c => c.Messages)
        .ThenInclude(m => m.Receiver) // Load the receiver
        .FirstOrDefaultAsync();

      if (conversation == default)
      {
        throw new KeyNotFoundException("Conversation with the given id doesn't exist.");
      }

      return conversation.Messages.Select(m => m.ToMessageDto(m.Files)).ToList();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to get all messages.");
      throw;
    }
  }

  public async Task<List<IConversationDto>> GetAllConversations(Guid userId)
  {
    try
    {
      if (!await userService.UserExistsAsync(userId))
        throw new KeyNotFoundException("User with that userid is not found!");

      var kvps = await appContext
        .ConversationMemberships.Where(cm => cm.UserId == userId)
        .Include(cm => cm.Conversation)
        .Include(cm => cm.User)
        .GroupBy(g => g.ConversationId)
        .ToDictionaryAsync(
          g => g.Key,
          g =>
            g.Where(cm => cm.User != null)
              .Select(cm => cm.User!.ToConversationProfileDto())
              .ToList()
        );

      var result = kvps.Select(e =>
        (IConversationDto)
          new ConversationDtoBase { ConversationId = e.Key, Participants = e.Value.ToList() }
      );

      return result.ToList();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to get all conversations.");
      throw;
    }
  }

  public async Task<MessageDto> CreateMessageAsync(CreateMessageDto createMessageDto)
  {
    using var transaction = await appContext.Database.BeginTransactionAsync();
    try
    {
      var conversationId = await GetConversationIdOrCreate(
        createMessageDto.SenderId,
        createMessageDto.ReceiverId
      );

      Guid messageId = Guid.NewGuid(); // Create a messageID to share to fileService to establish associations

      List<Models.Entities.File> files = [];
      foreach (var file in createMessageDto.Files ?? [])
      {
        files.Add(await fileService.CreateFileAsync(file, messageId, DiscriminatorTypes.Message)); // Discriminator set to Message!
      }

      var message = createMessageDto.ToMessage(conversationId);
      message.MessageId = messageId; // associate the messageId with the newly created message

      var result = await appContext.Messages.AddAsync(message);
      await appContext.SaveChangesAsync();

      await transaction.CommitAsync();

      return result.Entity.ToMessageDto(files);
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(); // Revert incase of errors
      logger.LogError($"{ex}: An error occured trying to create a message.");
      throw;
    }
  }

  /// <summary>
  /// Get the conversation ID between two users, if no conversation exists, creates one and returns the ID
  /// </summary>
  /// <param name="senderId"></param>
  /// <param name="receiverId"></param>
  /// <returns>ConversationId</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task<Guid> GetConversationIdOrCreate(Guid senderId, Guid receiverId)
  {
    using var transaction = await appContext.Database.BeginTransactionAsync();
    try
    {
      // Find common conversations between sender and receiver
      var commonConversation = await appContext
        .ConversationMemberships.Where(cm => cm.UserId == senderId || cm.UserId == receiverId)
        .GroupBy(cm => cm.ConversationId)
        .Where(g => g.Count() == 2) // Both sender and receiver must be in this conversation
        .Select(g => g.Key)
        .FirstOrDefaultAsync();

      // If they don't have conversation yet
      if (commonConversation == default)
      {
        // Create a new conversation
        var conversation = new Conversation();
        var conversationMemberships = new List<ConversationMembership>
        {
          new ConversationMembership
          {
            UserId = senderId,
            ConversationId = conversation.ConversationId
          },
          new ConversationMembership
          {
            UserId = receiverId,
            ConversationId = conversation.ConversationId
          }
        };
        await appContext.ConversationMemberships.AddRangeAsync(conversationMemberships); // create the conversation memberships
        await appContext.Conversations.AddAsync(conversation); // create the conversation
        commonConversation = conversation.ConversationId;
      }
      await appContext.SaveChangesAsync();
      transaction.Commit();
      return commonConversation;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      logger.LogError(ex, "An error occurred while getting the conversation ID.");
      throw;
    }
  }

  public async Task<IConversationDto> GetConversationAsync(Guid conversationId)
  {
    try
    {
      if (!await ConversationExistsAsync(conversationId))
        throw new KeyNotFoundException("Conversation with the given id doesn't exist.");

      var kvp = await appContext
        .ConversationMemberships.Where(cm => cm.ConversationId == conversationId)
        .Include(cm => cm.Conversation)
        .Include(cm => cm.User)
        .GroupBy(g => g.ConversationId)
        .ToDictionaryAsync(
          g => g.Key,
          g =>
            g.Where(cm => cm.User != null)
              .Select(cm => cm.User!.ToConversationProfileDto())
              .ToList()
        );

      return kvp.Select(c =>
          (IConversationDto)
            new ConversationDtoBase { ConversationId = c.Key, Participants = [.. c.Value] }
        )
        .First();
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<ICollection<IConversationDto>> GetAllConversations()
  {
    try
    {
      var kvps = await appContext
        .ConversationMemberships.Include(cm => cm.Conversation)
        .Include(cm => cm.User)
        .GroupBy(g => g.ConversationId)
        .ToDictionaryAsync(
          g => g.Key,
          g =>
            g.Where(cm => cm.User != null)
              .Select(cm => cm.User!.ToConversationProfileDto())
              .ToList()
        );

      var result = kvps.Select(e =>
        (IConversationDto)
          new ConversationDtoBase { ConversationId = e.Key, Participants = e.Value.ToList() }
      );

      return result.ToList();
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<bool> ConversationExistsAsync(Guid conversationId)
  {
    return await appContext.Conversations.FindAsync(conversationId) != null;
  }

  public async Task DeleteMessage(Guid messageId)
  {
    try
    {
      var message = await appContext.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId);

      if (message == default)
        throw new KeyNotFoundException("Message with the given id is not found!");

      appContext.Messages.Remove(message);

      await appContext.SaveChangesAsync();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to delete message.");
      throw;
    }
  }
}
