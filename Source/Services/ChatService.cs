using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HealthHub.Source.Services;

public class ChatService(
  ApplicationContext appContext,
  UserService userService,
  FileService fileService,
  ILogger<ChatService> logger
)
{
  public async Task GetAllMessages(Guid senderId, Guid receiverId) =>
    throw new NotImplementedException();

  public async Task<List<ConversationDto>> GetAllConversations(Guid userId)
  {
    try
    {
      if (!await userService.UserExistsAsync(userId))
        throw new KeyNotFoundException("User with that userid is not found!");

      var conversations = await appContext
        .Users.Where(u => u.UserId == userId)
        .SelectMany(u => u.Conversations)
        .Include(c => c.Messages)
        .ThenInclude(m => m.Files)
        .Select(c =>
          c.ToConversationDto(
            c.Messages,
            c.Messages.First(m => m.ConversationId == c.ConversationId).Files
          )
        )
        .ToListAsync();

      Console.WriteLine($"{string.Join(",", conversations)}");

      return conversations;
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
      var conversationId = await GetConversationId(
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
  public async Task<Guid> GetConversationId(Guid senderId, Guid receiverId)
  {
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
        return conversation.ConversationId;
      }

      return commonConversation;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred while getting the conversation ID.");
      throw;
    }
  }

  public async Task<ConversationDto> GetConversationAsync(Guid conversationId)
  {
    try
    {
      if (!await ConversationExists(conversationId))
        throw new KeyNotFoundException("Conversation with the given id doesn't exist.");

      return appContext
        .Conversations.Include(c => c.Messages)
        .ThenInclude(m => m.Files)
        .Select(c =>
          c.ToConversationDto(
            c.Messages,
            c.Messages.Where(m => m.ConversationId == c.ConversationId).Select(m => m.Files).First()
          )
        )
        .First();
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<List<Guid>> GetAllConversations()
  {
    try
    {
      return await appContext.Conversations.Select(c => c.ConversationId).ToListAsync();
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<bool> ConversationExists(Guid conversationId)
  {
    return await appContext.Conversations.FindAsync(conversationId) != null;
  }
}
