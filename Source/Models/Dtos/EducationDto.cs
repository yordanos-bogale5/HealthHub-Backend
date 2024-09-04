public record CreateEducationDto(
  Guid DoctorId,
  string Degree,
  string Institution,
  DateOnly StartDate,
  DateOnly EndDate
);

public record EditEducationDto(
  string? Degree,
  string? Institution,
  DateOnly? StartDate,
  DateOnly? EndDate
);

public record EducationDto(
  Guid EducationId,
  string Degree,
  string Institution,
  DateOnly StartDate,
  DateOnly EndDate,
  Guid DoctorId
);
