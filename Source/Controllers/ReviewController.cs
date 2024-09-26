using HealthHub.Source.Services.ReviewService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/reviews")]
public class ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
  : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> PostReview(CreateReviewDto createReviewDto)
  {
    try
    {
      throw new NotImplementedException();
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }
}
