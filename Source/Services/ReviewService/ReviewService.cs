using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Services.PaymentService;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Source.Services.ReviewService;

public class ReviewService(
  AppointmentService appointmentService,
  IPaymentService paymentService,
  ApplicationContext appContext,
  ILogger<ReviewService> logger
) : IReviewService
{
  public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
  {
    try
    {
      // Before creating review we must check for
      // If the patient has finished appointment with a doctor
      var review = await appContext.Reviews.AddAsync(createReviewDto.ToReview());
      await appContext.SaveChangesAsync();
      return review.Entity.ToReviewDto();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to create review.");
      throw;
    }
  }

  public async Task DeleteReviewAsync(Guid reviewId)
  {
    try
    {
      var review = await appContext.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);

      if (review == default)
        throw new KeyNotFoundException("Review with the given id not found. Cannot delete.");

      appContext.Reviews.Remove(review);

      await appContext.SaveChangesAsync();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to delete review.");
      throw;
    }
  }

  public async Task<ReviewDto> EditReviewAsync(EditReviewDto editReviewDto)
  {
    try
    {
      var review = await appContext.Reviews.FirstOrDefaultAsync(r =>
        r.ReviewId == editReviewDto.ReviewId
      );

      if (review == default)
        throw new KeyNotFoundException("Review with the given id not found. Cannot edit.");

      review.ReviewText = editReviewDto.ReviewText;
      review.StarRating = editReviewDto.StarRating;

      await appContext.SaveChangesAsync();

      return review.ToReviewDto();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to edit review.");
      throw;
    }
  }

  public async Task<ICollection<ReviewDto>> GetAllReviewsForDoctorAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);

      if (doctor == default)
        throw new KeyNotFoundException("Doctor with the given id is not found.");

      var reviews = await appContext.Reviews.Where(r => r.DoctorId == doctorId).ToListAsync();
      return reviews.Select(r => r.ToReviewDto()).ToList();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to edit review.");
      throw;
    }
  }

  public Task<ICollection<ReviewDto>> GetAllReviewsForPatientAsync(Guid patientId) =>
    throw new NotImplementedException();

  public Task<ReviewDto> GetReviewAsync(Guid reviewId) => throw new NotImplementedException();
}
