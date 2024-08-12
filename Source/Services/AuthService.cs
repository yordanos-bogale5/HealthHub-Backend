using HealthHub.Source;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.ViewModels;

namespace HealthHub.Source.Services;

/// <summary>
/// This Class is responsible for all Authentication related functionalities.
/// </summary>
/// <param name="appContext"></param>
/// <param name="emailService"></param>
/// <param name="renderingService"></param>
/// <param name="logger"></param>
public class AuthService(Data.AppContext appContext, EmailService emailService, RenderingService renderingService, ILogger<AuthService> logger)
{
  public async Task SendOtp(Guid userId)
  {
    try
    {
      // Generate OTP
      var otp = new Random().Next(100000, 999999);

      User? user = await appContext.Users.FindAsync(userId);
      logger.LogInformation($"FIRSTNAME : {user?.FirstName}");

      if (user == null)
      {
        logger.LogError("User with that id is not found!");
        throw new ArgumentException("User with that id is not found!");
      }

      appContext.Entry(user).Property(u => u.Otp).CurrentValue = otp;

      // Generate the Email Template with appropriate model fields
      var emailBody = await renderingService.RenderRazorPage("Source/Views/WelcomeEmail.cshtml", new WelcomeEmailModel()
      {
        Email = user.Email,
        Name = $"{user.FirstName} {user.LastName}",
        Otp = otp,
        SupportEmail = "healthhub.support@gmail.com"
      });

      // Send an OTP message to the users email
      await emailService.SendEmail(user.Email, $"{user.FirstName} {user.LastName}", "Verify Registration", emailBody);

      await appContext.SaveChangesAsync();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to send OTP");
      throw new Exception("Internal Error", ex);
    }
  }



}