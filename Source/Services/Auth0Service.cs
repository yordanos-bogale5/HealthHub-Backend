using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Services;

public class Auth0Service
{

  /// <summary>
  /// Responsible for creating a new user in the Auth0 database.
  /// </summary>
  /// <param name="userDto"></param>
  /// <returns></returns>
  public async Task<Guid> CreateUser(RegisterUserDto userDto)
  {
    return Guid.NewGuid();
  }

  /// <summary>
  /// Responsible for getting the management access token for making management API calls.
  /// </summary>
  /// <returns></returns>
  public static async Task<string> GetManagementApiToken()
  {
    return "";
  }
}