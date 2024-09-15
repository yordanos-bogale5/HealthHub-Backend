using AutoMapper;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class NotificationHub(ApplicationContext appContext, UserConnection userConnection) : Hub
{
  private string? _senderId;

  public override async Task OnConnectedAsync()
  {
    _senderId = "74d501f1-b888-41cc-acb9-230eaa17698e";
    userConnection.AddConnection(_senderId, Context.ConnectionId);
    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    if (_senderId != null)
    {
      userConnection.RemoveConnection(_senderId);
    }

    await base.OnDisconnectedAsync(exception);
  }

  /// <summary>
  /// Send notifications to the user specified by the userId
  /// </summary>
  /// <param name="message"></param>
  /// <param name="userId"></param>
  /// <param name="notificationType"></param>
  /// <returns></returns>
  public async Task SendNotification(string message, Guid userId, NotificationType notificationType)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(_senderId))
      {
        throw new FormatException("The userId cookie is missing or empty.");
      }

      var notification = new Notification
      {
        Message = message,
        NotificationType = notificationType,
        UserId = userId
      };

      await appContext.Notifications.AddAsync(notification);

      await appContext.SaveChangesAsync();

      var connectionId = userConnection.GetConnectionId(_senderId);

      if (connectionId != null)
      {
        await Clients
          .User(connectionId)
          .SendAsync(NotificationEvents.ReceiveNotification.ToString());
      }
      else
      {
        throw new FormatException("The connectionId is missing or empty.");
      }
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }
}
