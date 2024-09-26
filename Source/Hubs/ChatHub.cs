using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Services;
using HealthHub.Source.Services.ChatService;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Cms;
using Serilog;

namespace HealthHub.Source.Hubs
{
  public class ChatHub : Hub
  {
    private readonly IChatService _chatService;
    private readonly UserConnection _userConnection;
    private string? _senderId;

    public ChatHub(ChatService chatService, UserConnection userConnection)
    {
      _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
      _userConnection = userConnection ?? throw new ArgumentNullException(nameof(userConnection));
    }

    public override async Task OnConnectedAsync()
    {
      var httpContext = Context.GetHttpContext();
      _senderId = httpContext?.Request.Cookies[CookieDefaults.Profile.UserId];

      if (_senderId == null)
        throw new HubException("User is not logged in.");

      // Use the senderId to manage connection mapping
      // _senderId = "74d501f1-b888-41cc-acb9-230eaa17698e";
      _userConnection.AddConnection(_senderId, Context.ConnectionId);

      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
      if (_senderId != null)
      {
        _userConnection.RemoveConnection(_senderId);
      }

      await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a message to a user specified by the receiverId
    /// </summary>
    /// <param name="conversationId"></param>
    /// <param name="messageText"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public async Task SendMessage(
      [Guid] Guid conversationId,
      [MinLength(1, ErrorMessage = "Message text cannot be empty")] string? messageText = null,
      [ValidCreateFileList] List<CreateFileDto>? files = null
    )
    {
      // _senderId = "74d501f1-b888-41cc-acb9-230eaa17698e";
      if (string.IsNullOrWhiteSpace(_senderId))
      {
        throw new FormatException("The userId cookie is missing or empty.");
      }

      if (string.IsNullOrWhiteSpace(messageText) && (files == null || files.Count == 0))
      {
        throw new ArgumentException("Either message text or files must be provided.");
      }

      // var senderId = Context
      //   .GetHttpContext()
      //   ?.Request.Cookies[AuthDefaults.User.UserId]
      //   ?.ToString();
      Console.WriteLine($"\n\nhuh, {_senderId}");
      if (!Guid.TryParse(_senderId, out Guid senderGuid))
        throw new FormatException("The userId cookie is malformed. Not a valid guid.");

      var messagePayload = new CreateMessageDto(conversationId, senderGuid, messageText, files);

      // Store message in db
      var createdMessage = await _chatService.CreateMessageAsync(messagePayload);

      var connId = _userConnection.GetConnectionId(_senderId);
      if (connId != null)
      {
        await Clients
          .Client(connId)
          .SendAsync(ChatEvents.ReceiveMessage.ToString(), createdMessage);
      }
    }

    public async Task SendMessageAll(string user, string message)
    {
      await Clients.All.SendAsync("ReceiveMessageAll", user, message);
    }
  }
}
