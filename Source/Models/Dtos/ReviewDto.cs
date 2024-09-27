using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;

public record ReviewDto
{
  public required Guid ReviewId { get; set; }
  public required Guid DoctorId { get; set; }
  public required Guid? PatientId { get; set; }
  public required decimal StarRating { get; set; }
  public required string ReviewText { get; set; }
};

public record CreateReviewDto
{
  [Required]
  public required Guid DoctorId { get; set; }

  [Required]
  public required Guid PatientId { get; set; }

  [Required]
  [StarRating]
  [Range(0, 5)]
  public required decimal StarRating { get; set; }

  [Required]
  public required string ReviewText { get; set; }
};

public record EditReviewDto
{
  [Required]
  public required Guid ReviewId { get; set; }

  [Required]
  [StarRating]
  [Range(0, 5)]
  public required decimal StarRating { get; set; }

  [Required]
  public required string ReviewText { get; set; }
};
