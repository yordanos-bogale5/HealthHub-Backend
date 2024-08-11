namespace HealthHub.Source.Config;

public class AppConfig(IConfiguration configuration)
{
  public string? DatabaseConnection { get; set; } = configuration["DB_CONNECTION"];
  public string? MailSenderEmail { get; set; } = configuration["MAIL_SENDER_EMAIL"];
  public string? MailSenderPassword { get; set; } = configuration["MAIL_SENDER_PASSWORD"];
  public string? Auth0Domain { get; set; } = configuration["AUTH0_DOMAIN"];
  public string? Auth0Audience { get; set; } = configuration["AUTH0_AUDIENCE"];
  public string? Auth0ClientId { get; set; } = configuration["AUTH0_CLIENT_ID"];
  public string? Auth0ClientSecret { get; set; } = configuration["AUTH0_CLIENT_SECRET"];
}