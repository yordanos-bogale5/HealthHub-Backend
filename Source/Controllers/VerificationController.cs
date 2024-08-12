using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class VerificiationController(ILogger<VerificiationController> logger) : ControllerBase
{


  [HttpGet("email")]
  public async Task<IActionResult> VerifyEmailAsync(bool success, string message)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      logger.LogInformation($"Auth0 Callback \nSuccess: {success} \nMessage: {message}");
      return Ok();

    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Verify Email");
      throw new Exception("Internal Server Error ", ex);
    }
  }
}