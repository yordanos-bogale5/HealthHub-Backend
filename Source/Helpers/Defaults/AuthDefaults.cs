using Microsoft.Identity.Client;

namespace HealthHub.Source.Helpers.Defaults;

public static class AuthDefaults
{
  public const string AccessToken = "access_token";
  public const string Authorization = "authorization";
}

public static class CookieDefaults
{
  public static class Profile
  {
    public const string UserId = "user_id";
    public const string DateOfBirth = "date_of_birth";
    public const string FirstName = "first_name";
    public const string LastName = "last_name";
    public const string Phone = "phone";
    public const string Role = "role";
  }
}
