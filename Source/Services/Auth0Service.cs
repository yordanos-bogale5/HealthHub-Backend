using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Enums;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;

namespace HealthHub.Source.Services;

public class Auth0Service(AppConfig appConfig, ILogger<Auth0Service> logger)
{
  /// <summary>
  /// Responsible for creating a new user in the Auth0 database.
  /// </summary>
  /// <param name="userDto"></param>
  /// <param name="userId"></param>
  /// <returns>Newly created auth0 UserID</returns>
  public async Task<Auth0UserDto?> CreateUserAsync(RegisterUserDto userDto, Guid userId)
  {
    try
    {
      var userPayload = new
      {
        email = userDto.Email,
        password = userDto.Password,
        connection = "Username-Password-Authentication",
        user_metadata = new
        {
          userId,
          firstName = userDto.FirstName,
          lastName = userDto.LastName,
          role = userDto.Role.ToString(),
          phone = userDto.Phone,
          gender = userDto.Gender,
          dateOfBirth = userDto.DateOfBirth,
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
        logger.LogError(response.Content, $"Auth0 Create User Error\n\n");
        throw new Exception("Failed to create user in Auth0");
      }

      logger.LogInformation($"\n\nAuth0 Create User Success:\n {response.Content}");

      var userData = JsonSerializer.Deserialize<JsonElement>(response.Content!);

      return new Auth0UserDto(
        userData.GetProperty("user_id").GetString()!,
        userData.GetProperty("picture").GetString()!,
        userData.GetProperty("email_verified").GetBoolean()
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to create user in Auth0");
      throw;
    }
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
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to delete user in Auth0");
      throw;
    }
  }

  public async Task<Auth0LoginDto?> LoginUserAsync(LoginUserDto loginUserDto, string auth0UserId)
  {
    try
    {
      var token = await GetManagementApiTokenAsync();

      var client = new RestClient($"{appConfig.Auth0Authority}/oauth/token");

      var request = new RestRequest() { Method = Method.Post };

      request.AddHeader("content-type", "application/json");
      request.AddHeader("Authorization", $"Bearer {token}");

      request.AddBody(
        new
        {
          grant_type = "password",
          username = loginUserDto.Email,
          password = loginUserDto.Password,
          audience = appConfig.Auth0Audience,
          client_id = appConfig.Auth0ClientId,
          client_secret = appConfig.Auth0ClientSecret
        }
      );

      var response = await client.ExecuteAsync(request);

      logger.LogInformation($"\n\nAuth0 Login User Response:\n {response.Content}");

      var responseData = JsonSerializer.Deserialize<JsonElement>(response.Content!);

      if (!response.IsSuccessStatusCode)
      {
        logger.LogError(response.Content);
        throw new Exception(responseData.GetProperty("error_description").ToString());
      }

      var profile = await GetUserProfileAsync(auth0UserId);

      return new Auth0LoginDto(
        responseData.GetProperty("access_token").ToString(),
        responseData.GetProperty("expires_in").GetInt32(),
        profile
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to login user in Auth0");
      throw;
    }
  }

  public async Task<bool?> IsEmailVerified(string userId)
  {
    try
    {
      var token = await GetManagementApiTokenAsync();

      var client = new RestClient($"{appConfig.Auth0Authority}/api/v2/users/{userId}");
      var request = new RestRequest() { Method = Method.Get };

      request.AddHeader("Authorization", $"Bearer {token}");

      var response = await client.ExecuteAsync(request);

      if (!response.IsSuccessStatusCode)
      {
        logger.LogError(response.Content, $"Auth0 Get User Error for email verification");
        throw new Exception("Failed to get user in Auth0 for email verification");
      }

      return JsonSerializer
        .Deserialize<JsonElement>(response.Content!)
        .GetProperty("email_verified")
        .GetBoolean();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to verify email in Auth0");
      throw;
    }
  }

  public async Task<Auth0ProfileDto> GetUserProfileAsync(string auth0Id)
  {
    try
    {
      var token = await GetManagementApiTokenAsync();

      var url = $"{appConfig.Auth0Authority}/api/v2/users/{auth0Id}";
      var restClient = new RestClient(url);

      var restRequest = new RestRequest() { Method = Method.Get };
      restRequest.AddHeader("Authorization", $"Bearer {token}");

      var response = await restClient.ExecuteAsync(restRequest);

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception("Failed to get auth0 user profile");
      }

      var jsonData = JsonSerializer.Deserialize<JsonElement>(response.Content!);

      logger.LogInformation(response.Content);

      if (!jsonData.TryGetProperty("user_metadata", out var userMetaData))
      {
        throw new Exception("User metadata is not present in the response");
      }

      // Use TryGetProperty to handle missing fields gracefully

      var userId = userMetaData.TryGetProperty("userId", out var userIdElement)
        ? Guid.TryParse(userIdElement.GetString(), out var userGuid)
          ? userGuid
          : Guid.Empty
        : Guid.Empty;

      var firstName = userMetaData.TryGetProperty("firstName", out var firstNameElement)
        ? firstNameElement.GetString()
        : "";
      var lastName = userMetaData.TryGetProperty("lastName", out var lastNameElement)
        ? lastNameElement.GetString()
        : "";
      var role = userMetaData.TryGetProperty("role", out var roleElement)
        ? roleElement.GetString()
        : "";
      var phone = userMetaData.TryGetProperty("phone", out var phoneElement)
        ? phoneElement.GetString()
        : "";
      var gender = userMetaData.TryGetProperty("gender", out var genderElement)
        ? genderElement.GetString()
        : "";
      var dateOfBirth = userMetaData.TryGetProperty("dateOfBirth", out var dateOfBirthElement)
        ? dateOfBirthElement.GetString()
        : "";

      return new Auth0ProfileDto
      {
        UserId = userId,
        FirstName = firstName,
        LastName = lastName,
        Role = string.IsNullOrEmpty(role) ? default : role.ConvertToEnum<Role>(),
        Phone = phone,
        Gender = string.IsNullOrEmpty(gender) ? default : gender.ConvertToEnum<Gender>(),
        DateOfBirth = dateOfBirth
      };
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: AN error occured trying to get auth0 profile");
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
    // logger.LogInformation($"Auth0 Get Management API Token Request:\n {url}");

    var request = new RestRequest() { Method = Method.Post };

    request.AddHeader("content-type", "application/x-www-form-urlencoded");
    request.AddParameter("grant_type", "client_credentials");
    request.AddParameter("client_id", clientId);
    request.AddParameter("client_secret", clientSecret);
    request.AddParameter("audience", audience);

    RestResponse response = await client.ExecuteAsync(request);
    // logger.LogInformation($"\n\nThis is the Management Api Response: {response.Content}");
    if (!response.IsSuccessStatusCode)
    {
      logger.LogError(response.Content, $"Auth0 Get Management API Token Error");

      throw new Exception("Failed to get management API token");
    }

    var tokenData = JsonSerializer.Deserialize<JsonElement>(response.Content!);
    return tokenData.GetProperty("access_token").GetString()!;
  }
}
