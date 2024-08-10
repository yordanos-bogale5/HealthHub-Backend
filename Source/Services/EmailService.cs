using MailKit.Net.Smtp;
using MimeKit;

namespace HealthHub.Source.Services;

public class EmailService(IConfiguration configuration)
{
  /// <summary>
  /// Function used to send email to people.
  /// </summary>
  /// <param name="toEmail"></param>
  /// <param name="toName"></param>
  /// <param name="subject"></param>
  /// <param name="body"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task SendEmail(string toEmail, string toName, string subject, string body)
  {
    try
    {
      var message = new MimeMessage();

      // Setup the Contents of your email address
      message.From.Add(new MailboxAddress("HealthHub Inc.", "dagtef@gmail.com"));
      message.To.Add(new MailboxAddress(toName, toEmail));
      message.Subject = subject;
      message.Body = new TextPart("html")
      {
        Text = body
      };


      // Send the MimeMessage to the specified Email Address using the SmtpClient 
      using (var client = new SmtpClient())
      {
        Console.WriteLine($"--CONFIGURATOIN-{configuration}");

        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(configuration["MAIL_SENDER_EMAIL"], configuration["MAIL_SENDER_PASSWORD"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
      }
    }
    catch (System.Exception ex)
    {
      Console.WriteLine($"{ex}");
      throw new Exception("Error sending email ", ex);
    }
  }
}