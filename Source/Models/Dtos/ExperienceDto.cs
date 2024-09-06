using System.ComponentModel.DataAnnotations;

public record EditExperienceDto(
  string? Institution,
  string? StartDate,
  string? EndDate,
  string? Description
);

public record CreateExperienceDto(
  [Required] string Institution,
  [Required] string StartDate,
  string? EndDate,
  string? Description
);

// This is what we return after creating experience
public record ExperienceDto(
  Guid ExperienceId,
  string Institution,
  DateOnly StartDate,
  DateOnly? EndDate,
  string? Description,
  Guid DoctorId
);
