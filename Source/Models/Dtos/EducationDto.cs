using System.ComponentModel.DataAnnotations;

namespace HealthHub.Source.Models.Dtos;

public record CreateEducationDto(
  [Required] string Degree,
  [Required] string Institution,
  [Required] string StartDate,
  [Required] string EndDate
);

public record EditEducationDto(
  string? Degree,
  string? Institution,
  string? StartDate,
  string? EndDate
);

public record EducationDto(
  Guid EducationId,
  string Degree,
  string Institution,
  DateOnly StartDate,
  DateOnly EndDate,
  Guid DoctorId
);
