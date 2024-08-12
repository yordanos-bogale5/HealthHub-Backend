using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using HealthHub.Source.Config;
using HealthHub.Source.Models.Dtos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;

namespace HealthHub.Source.Services;

public class Auth0Service(AppConfig appConfig, ILogger<Auth0Service> logger)
{

  /// <summary>
  /// Responsible for creating a new user in the Auth0 database.
  /// </summary>
  /// <param name="userDto"></param>
  /// <returns>Newly created auth0 UserID</returns>
  public async Task<Auth0UserDto?> CreateUserAsync(RegisterUserDto userDto)
  {

    var userPayload = new
    {
      email = userDto.Email,
      password = userDto.Password,
      connection = "Username-Password-Authentication",
      user_metadata = new
      {
        firstName = userDto.FirstName,
        lastName = userDto.LastName,
      }
    };

    var token = await GetManagementApiTokenAsync();

    var client = new RestClient($"{appConfig.Auth0Authority}/api/v2/users");

    var request = new RestRequest() { Method = Method.Post };

    request.AddHeader("content-type", "application/json");
    request.AddHeader("Authorization", $"Bearer {token}");
    request.AddJsonBody(userPayload);

    RestResponse response = await client.ExecuteAsync(request);


    if (response.StatusCode != System.Net.HttpStatusCode.Created)
    {
      logger.LogError(response.Content, $"Auth0 Create User Error");
      throw new Exception("Failed to create user in Auth0");
    }

    logger.LogInformation($"Auth0 Create User Success:\n {response.Content}");


    var userData = JsonSerializer.Deserialize<JsonElement>(response.Content!);

    return new Auth0UserDto(
      userData.GetProperty("user_id").GetString()!,
      userData.GetProperty("picture").GetString()!,
      userData.GetProperty("email_verified").GetBoolean()
    );
  }

  public async Task DeleteUserAsync(string userId)
  {
    try
    {
      var token = await GetManagementApiTokenAsync();

      var client = new RestClient($"{appConfig.Auth0Authority}/api/v2/users/{userId}");
      var request = new RestRequest() { Method = Method.Delete };

      request.AddHeader("Authorization", $"Bearer {token}");

      var response = await client.ExecuteAsync(request);

      if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
      {
        logger.LogError(response.Content, $"Auth0 Delete User Error");
        throw new Exception("Failed to delete user in Auth0");
      }

      logger.LogInformation($"Auth0 Delete User Success:\n {response.Content}");
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to delete user in Auth0");
      throw;
    }
  }

  /// <summary>
  /// Responsible for getting the management access token for making management API calls.
  /// </summary>
  /// <returns></returns>
  public async Task<string> GetManagementApiTokenAsync()
  {
    var clientId = appConfig.Auth0ClientId;
    var clientSecret = appConfig.Auth0ClientSecret;
    var audience = appConfig.Auth0Audience;
    var url = $"{appConfig.Auth0Authority}/oauth/token";

    var client = new RestClient(url);
    logger.LogInformation($"Auth0 Get Management API Token Request:\n {url}");

    var request = new RestRequest() { Method = Method.Post };

    request.AddHeader("content-type", "application/x-www-form-urlencoded");
    request.AddParameter("grant_type", "client_credentials");
    request.AddParameter("client_id", clientId);
    request.AddParameter("client_secret", clientSecret);
    request.AddParameter("audience", audience);

    RestResponse response = await client.ExecuteAsync(request);

    if (response.StatusCode != System.Net.HttpStatusCode.OK)
    {
      logger.LogError(response.Content, $"Auth0 Get Management API Token Error");

      throw new Exception("Failed to get management API token");
    }

    logger.LogInformation($"Auth0 Get Management API Token Success:\n {response.Content}");

    var tokenData = JsonSerializer.Deserialize<JsonElement>(response.Content!);
    return tokenData.GetProperty("access_token").GetString()!;
  }
}