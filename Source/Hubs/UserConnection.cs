public class UserConnection
{
  private readonly Dictionary<string, string> UserConnectionMap = new();

  public void AddConnection(string userId, string connectionId)
  {
    UserConnectionMap[userId] = connectionId;
  }

  public void RemoveConnection(string userId)
  {
    UserConnectionMap.Remove(userId);
  }

  public string? GetConnectionId(string userId)
  {
    if (UserConnectionMap.TryGetValue(userId, out var connectionId))
      return connectionId;
    return null;
  }
}
