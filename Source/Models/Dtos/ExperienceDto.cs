public record EditExperienceDto(
  string? Institution,
  DateOnly? StartDate,
  DateOnly? EndDate,
  string? Description
);

public record CreateExperienceDto(
  Guid DoctorId,
  string Institution,
  DateOnly StartDate,
  DateOnly EndDate,
  string? Description
);

// This is what we return after creating experience
public record ExperienceDto(
  Guid ExperienceId,
  string Institution,
  DateOnly StartDate,
  DateOnly EndDate,
  string? Description,
  Guid DoctorId
);
