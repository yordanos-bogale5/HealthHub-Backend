using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using HealthHub.Source.Config;
using HealthHub.Source.Models.Dtos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;

namespace HealthHub.Source.Services;

public class Auth0Service(AppConfig appConfig)
{

  /// <summary>
  /// Responsible for creating a new user in the Auth0 database.
  /// </summary>
  /// <param name="userDto"></param>
  /// <returns>Newly created auth0 UserID</returns>
  public async Task<string?> CreateUserAsync(RegisterUserDto userDto)
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
    Console.WriteLine("THIS IS THE TOKEN " + token);

    var client = new RestClient($"{appConfig.Auth0Authority}/api/v2/users");

    var request = new RestRequest() { Method = Method.Post };

    request.AddHeader("content-type", "application/json");
    request.AddHeader("Authorization", $"Bearer {token}");
    request.AddJsonBody(userPayload);

    RestResponse response = await client.ExecuteAsync(request);


    if (response.StatusCode != System.Net.HttpStatusCode.Created)
    {
      Console.WriteLine($"Auth0 Create User Error:\n {response.Content}");
      throw new Exception("Failed to create user in Auth0");
    }

    Console.WriteLine($"Auth0 Create User Success:\n {response.Content}");


    var userData = JsonSerializer.Deserialize<JsonElement>(response.Content!);

    return userData.GetProperty("user_id").GetString();
  }

  public async Task<Guid> DeleteUserAsync(Guid userId)
  {
    var token = await GetManagementApiTokenAsync();
    Console.WriteLine("THIS IS THE TOKEN " + token);

    var client = new RestClient($"{appConfig.Auth0Authority}/api/v2/users/{userId}");
    var request = new RestRequest() { Method = Method.Delete };

    request.AddHeader("Authorization", $"Bearer {token}");

    var response = await client.ExecuteAsync(request);

    if (response.StatusCode != System.Net.HttpStatusCode.OK)
    {
      Console.WriteLine($"Auth0 Delete User Error:\n {response.Content}");
      throw new Exception("Failed to delete user in Auth0");
    }

    Console.WriteLine($"Auth0 Delete User Success:\n {response.Content}");
    return userId;
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
    Console.WriteLine($"Auth0 Get Management API Token Request:\n {url}");

    var request = new RestRequest() { Method = Method.Post };

    request.AddHeader("content-type", "application/x-www-form-urlencoded");
    request.AddParameter("grant_type", "client_credentials");
    request.AddParameter("client_id", clientId);
    request.AddParameter("client_secret", clientSecret);
    request.AddParameter("audience", audience);

    RestResponse response = await client.ExecuteAsync(request);

    if (response.StatusCode != System.Net.HttpStatusCode.OK)
    {
      Console.WriteLine($"Auth0 Get Management API Token Error:\n {response.Content}");

      throw new Exception("Failed to get management API token");
    }

    Console.WriteLine($"Auth0 Get Management API Token Success:\n {response.Content}");

    var tokenData = JsonSerializer.Deserialize<JsonElement>(response.Content!);
    return tokenData.GetProperty("access_token").GetString()!;
  }
}