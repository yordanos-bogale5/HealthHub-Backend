using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chats")]
public class ChatController(ChatService chatService, ILogger<ChatController> logger)
  : ControllerBase
{
  [HttpGet("conversations/users/{userId}")]
  public async Task<IActionResult> GetConversations([FromRoute] [Required] [Guid] Guid userId)
  {
    try
    {
      logger.LogInformation($"\n\n{userId}");
      var result = await chatService.GetAllConversations(userId);

      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to get all conversations.");
      throw;
    }
  }

  [HttpGet("conversations/{userId}")]
  public async Task<IActionResult> GetConversation(
    [FromRoute] [Required] [Guid] Guid conversationId
  )
  {
    try
    {
      var result = await chatService.GetConversationAsync(conversationId);

      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to get all conversations.");
      throw;
    }
  }

  [HttpGet("conversations/all")]
  public async Task<IActionResult> GetAllConversations()
  {
    try
    {
      var result = await chatService.GetAllConversations();

      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to get all conversations.");
      throw;
    }
  }
}
