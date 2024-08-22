using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Validation;

public static class ValidationHelper
{
  public static bool BeAValidGuid(string? id)
  {
    if (id == null)
      return false;
    return Guid.TryParse(id, out _);
  }

  public static bool BeAValidDateTimeString(string? date)
  {
    if (date == null)
      return false;
    return DateTime.TryParse(date, out _);
  }

  public static bool BeAValidAppointmentType(string? appointmentTypeString)
  {
    if (appointmentTypeString == null)
      return false;
    return Enum.TryParse<AppointmentType>(appointmentTypeString, out _);
  }

  public static bool BeNotPastDate(string? date)
  {
    if (date == null)
      return false;
    return DateTime.TryParse(date, out var parsedDate) && parsedDate.Date >= DateTime.UtcNow.Date;
  }

  public static bool BeAtLeast18YearsOldFromString(string? dateString)
  {
    if (dateString == null)
      return false;
    if (!DateTime.TryParse(dateString, out var date))
      return false;

    var today = DateTime.Today;
    var age = today.Year - date.Year;

    if (date > today.AddYears(-age))
      age--;

    return age >= 18;
  }

  public static bool BeValidRole(string? roleString)
  {
    if (roleString == null)
      return false;
    return Enum.TryParse<Role>(roleString, true, out _);
  }

  public static bool BeValidGender(string? genderString)
  {
    if (genderString == null)
      return false;
    return Enum.TryParse<Gender>(genderString, true, out _);
  }
}
