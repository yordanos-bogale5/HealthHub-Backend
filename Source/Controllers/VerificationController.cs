using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/verify")]
public class VerificationController(UserService userService, ILogger<VerificationController> logger) : ControllerBase
{

  /// <summary>
  /// This endpoint is responsible for verifying a user's email.
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  [HttpGet("email/{userId}")]
  public async Task<IApiResponse<bool>> VerifyEmailAsync(Guid userId)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        return new ApiResponse<bool>(false, "Invalid Model State", false);
      }

      var result = await userService.CheckEmailVerified(userId);

      return new ApiResponse<bool>(result.Success, result.Message, result.Data ?? false);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Check Email Verification");
      throw new Exception("Internal Server Error ", ex);
    }
  }
}