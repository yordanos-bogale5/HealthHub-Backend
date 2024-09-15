using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

public class DoctorService(
  ApplicationContext appContext,
  SpecialityService specialityService,
  DoctorSpecialityService doctorSpecialityService,
  ILogger<DoctorService> logger
)
{
  public async Task<Doctor> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
  {
    try
    {
      var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
      var doctor = doctorResult.Entity;

      // Create all educations for the doctor
      foreach (CreateEducationDto createEducationDto in createDoctorDto.Educations)
      {
        await CreateEducationAsync(createEducationDto, doctor.DoctorId);
      }

      // Create all experiences for the doctor
      foreach (CreateExperienceDto createExperienceDto in createDoctorDto.Experiences)
      {
        await CreateExperienceAsync(createExperienceDto, doctor.DoctorId);
      }

      await appContext.SaveChangesAsync();
      return doctor;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Doctor");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorProfileDto>>> GetAllDoctors()
  {
    try
    {
      List<DoctorProfileDto> doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Include(d => d.Educations)
        .Include(d => d.Experiences)
        .Select(d =>
          d.ToDoctorProfileDto(
            d.User,
            d.DoctorAvailabilities,
            d.DoctorSpecialities.Where(ds => ds.Speciality != null)
              .Select(ds => ds.Speciality!)
              .ToList(),
            d.Educations,
            d.Experiences
          )
        )
        .ToListAsync();

      return new ServiceResponse<List<DoctorProfileDto>>(
        true,
        200,
        doctorUsers,
        "All Doctors Retrieved!"
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"Failed to get all Doctors in User Service: {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsBySpecialityAsync(
    string specialityName
  )
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(ds.Speciality.SpecialityName, $"%{specialityName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with speciality name retrieved"
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"Failed to get doctors by speciality in doctor service {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByNameAsync(string doctorName)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d =>
          d.DoctorSpecialities.Any(ds =>
            EF.Functions.Like(d.User.FirstName + " " + d.User.LastName, $"%{doctorName}%")
          )
        )
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with name retrieved"
      );
    }
    catch (Exception ex)
    {
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByGenderAsync(Gender gender)
  {
    try
    {
      var doctorUsers = await appContext
        .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Where(d => d.User.Gender == gender)
        .Select(d => d.ToDoctorDto(d.User, d.DoctorSpecialities))
        .ToListAsync();

      return new ServiceResponse<List<DoctorDto>>(
        true,
        200,
        doctorUsers,
        "Doctors with gender retrieved"
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"Getting doctors by gender failed: {ex}");
      throw;
    }
  }

  /// <summary>
  /// Gets a doctor with doctorId and returns it with User, DoctorSpecialities and Speciality fields Populated
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>Populated Doctor Model</returns>
  public async Task<ServiceResponse<Doctor>> GetDoctorAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .SingleOrDefaultAsync(d => d.DoctorId == doctorId);

      if (doctor == null)
      {
        return new ServiceResponse<Doctor>(false, 404, null, "Doctor not found");
      }

      return new ServiceResponse<Doctor>(true, 200, doctor, "Doctor found");
    }
    catch (Exception ex)
    {
      logger.LogError($"Failed to get doctor by id {ex}");
      throw;
    }
  }

  /// <summary>
  /// Checks if a doctor exists
  /// </summary>
  /// <param name="doctorId"></param>
  /// <returns>True if doctor exists, otherwise False </returns>
  public async Task<bool> CheckDoctorExistsAsync(Guid doctorId)
  {
    try
    {
      var doctor = await appContext.Doctors.FindAsync(doctorId);
      return doctor != null;
    }
    catch (Exception ex)
    {
      logger.LogError($"Failed to check if doctor exists {ex}");
      throw;
    }
  }

  public async Task<DoctorProfileDto> EditDoctorProfileAsync(
    EditDoctorProfileDto editDoctorProfileDto
  )
  {
    try
    {
      var doctor = await appContext
        .Doctors.Include(d => d.User)
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .Include(d => d.DoctorAvailabilities)
        .Include(d => d.Educations)
        .Include(d => d.Experiences)
        .FirstOrDefaultAsync(d => d.UserId == editDoctorProfileDto.UserId);

      if (doctor == null)
      {
        throw new BadHttpRequestException("Doctor with that userId is not present.");
      }

      /* Creating the New Specialtities */
      var specialities =
        editDoctorProfileDto.Specialitites != null
          ? await specialityService.CreateSpecialitiesAsync(
            editDoctorProfileDto.Specialitites.ToSpecialityList(doctor.DoctorId)
          )
          : null;

      // Create doctor specialities based on the new specialities
      var docSpecs =
        specialities != null
          ? await doctorSpecialityService.CreateDoctorSpecialitiesAsync(
            specialities.Select(s => s.ToCreateDoctorSpecialityDto(doctor)).ToList()
          )
          : null;

      /* Creating the New Doctor Availabilities */
      var availabilities =
        editDoctorProfileDto.Availabilities != null
          ? await AddDoctorAvailabilityAsync(editDoctorProfileDto.Availabilities, doctor)
          : null;

      /* Perform Updates */
      doctor.DoctorSpecialities = docSpecs ?? doctor.DoctorSpecialities;

      doctor.Qualifications = editDoctorProfileDto.Qualifications ?? doctor.Qualifications;

      doctor.DoctorStatus =
        editDoctorProfileDto.DoctorStatus != null
          ? editDoctorProfileDto.DoctorStatus.ConvertToEnum<DoctorStatus>()
          : doctor.DoctorStatus;

      doctor.Biography = editDoctorProfileDto.Biography ?? doctor.Biography;

      doctor.DoctorAvailabilities =
        availabilities != null
          ? availabilities ?? doctor.DoctorAvailabilities
          : doctor.DoctorAvailabilities;

      if (editDoctorProfileDto.Educations != null) // If user has provided educations to be edited
      {
        await DeleteAllEducationsAsync(doctor.DoctorId); // Delete all educations

        foreach (EditEducationDto edDto in editDoctorProfileDto.Educations) // create new educations based on the request details
        {
          await CreateEducationAsync(
            new CreateEducationDto(
              edDto.Degree!,
              edDto.Institution!,
              edDto.StartDate!,
              edDto.EndDate!
            ),
            doctor.DoctorId
          );
        }
      }

      if (editDoctorProfileDto.Experiences != null)
      {
        await DeleteAllExperiencesAsync(doctor.DoctorId); // Delete all experiences

        foreach (EditExperienceDto edDto in editDoctorProfileDto.Experiences)
        {
          await CreateExperienceAsync(
            new CreateExperienceDto(
              edDto.Institution!,
              edDto.StartDate!,
              edDto.EndDate!,
              edDto.Description
            ),
            doctor.DoctorId
          );
        }
      }

      await appContext.SaveChangesAsync();

      return doctor.ToDoctorProfileDto(
        doctor.User,
        doctor.DoctorAvailabilities,
        doctor.DoctorSpecialities.Where(s => s != null).Select(ds => ds.Speciality!).ToList(),
        doctor.Educations,
        doctor.Experiences
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to edit doctor.");
      throw;
    }
  }

  /// <summary>
  /// Creates doctor availability entries given a List of tuple having (Day, StartTime, EndTime)
  /// </summary>
  /// <param name="doctorAvailabilities"></param>
  /// <param name="doctor"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<List<DoctorAvailability>> AddDoctorAvailabilityAsync(
    List<AvailabilityDto> doctorAvailabilities,
    Doctor doctor
  )
  {
    try
    {
      List<DoctorAvailability> dbDoctorAvailabilities = [];

      if (doctorAvailabilities.Count == 0)
      {
        foreach (
          DayOfWeek day in new List<DayOfWeek>
          {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
          }
        )
        {
          dbDoctorAvailabilities.Add(
            new DoctorAvailability
            {
              Doctor = doctor,
              DoctorId = doctor.DoctorId,
              AvailableDay = day,
              StartTime = new TimeOnly(10, 0),
              EndTime = new TimeOnly(17, 0)
            }
          );
        }
      }
      else
      {
        foreach (var (day, startTime, endTime) in doctorAvailabilities)
        {
          dbDoctorAvailabilities.Add(
            new DoctorAvailability
            {
              Doctor = doctor,
              DoctorId = doctor.DoctorId,
              AvailableDay = day.ConvertToEnum<DayOfWeek>(),
              StartTime = TimeOnly.Parse(startTime),
              EndTime = TimeOnly.Parse(endTime)
            }
          );
        }
      }

      await appContext.DoctorAvailabilities.AddRangeAsync(dbDoctorAvailabilities);
      await appContext.SaveChangesAsync();

      return dbDoctorAvailabilities;
    }
    catch (Exception ex)
    {
      logger.LogError($"An error occured when trying to add doctor availability {ex}");
      throw;
    }
  }

  /// <summary>
  /// To Check if a doctor is available at the appointmentDay and Time
  /// </summary>
  /// <param name="doctorId"></param>
  /// <param name="appointmentDay"></param>
  /// <param name="appointmentTime"></param>
  /// <param name="appointmentTimeSpan"></param>
  /// <returns>True if he/she is available, otherwise false.</returns>
  public async Task<bool> CheckDoctorAvailabilityAsync(
    Guid doctorId,
    DayOfWeek appointmentDay,
    TimeOnly appointmentTime,
    TimeSpan appointmentTimeSpan
  )
  {
    try
    {
      var result = await appContext.DoctorAvailabilities.ToListAsync();
      return result.Any(da =>
        da.DoctorId == doctorId
        && da.AvailableDay == appointmentDay
        && da.StartTime <= appointmentTime
        && appointmentTime.Add(appointmentTimeSpan) <= da.EndTime
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: Failed to Check doctor availability");
      throw;
    }
  }

  public async Task<Dictionary<DayOfWeek, List<TimeRange>>> GetDoctorAvailabilitiesAsync(
    Guid doctorId
  )
  {
    try
    {
      if (!await CheckDoctorExistsAsync(doctorId))
      {
        throw new KeyNotFoundException("Doctor is not found!");
      }

      var dayTimesMap = new Dictionary<DayOfWeek, List<TimeRange>>();

      // The below algorithm
      // Gets doctor available DayOfWeek with start time and end time
      /*
        {
          Monday = [10,17]
          Tuesday = [12,16]
          Wednesday = [14,16]
        }
      */

      await appContext
        .DoctorAvailabilities.Where(da => da.DoctorId == doctorId)
        .ForEachAsync(da =>
        {
          if (!dayTimesMap.ContainsKey(da.AvailableDay))
            dayTimesMap[da.AvailableDay] = [];

          dayTimesMap[da.AvailableDay].Add(new TimeRange(da.StartTime, da.EndTime));
        });

      return dayTimesMap;
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex} : An error occured trying to get doctor availabilities");
      throw;
    }
  }

  public async Task<Dictionary<DayOfWeek, List<TimeRange>>> GetDoctorAppointmentTimesAsync(
    Guid doctorId
  )
  {
    try
    {
      var docAppTimes = new Dictionary<DayOfWeek, List<TimeRange>>();

      var appointments = await appContext
        .Appointments.Where(ap => ap.DoctorId == doctorId)
        .ToListAsync();

      foreach (var ap in appointments)
      {
        DayOfWeek day = ap.AppointmentDate.DayOfWeek.ToString().ConvertToEnum<DayOfWeek>();
        if (!docAppTimes.ContainsKey(day))
          docAppTimes[day] = new List<TimeRange>();
        Console.WriteLine(day);
        docAppTimes[day]
          .Add(new TimeRange(ap.AppointmentTime, ap.AppointmentTime.Add(ap.AppointmentTimeSpan)));
      }

      return docAppTimes;
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An Error occured trying to get doctor appointment times");
      throw;
    }
  }

  /// <summary>
  /// Retrieves the profile of the doctor specified with the userId
  /// </summary>
  /// <param name="userId"></param>
  /// <returns>A <see cref="DoctorProfileDto"/> representing the doctor's profile.</returns>
  /// <exception cref="KeyNotFoundException"/>Thrown when no doctor is found with the specified userId.<exception/>
  public async Task<DoctorProfileDto> GetDoctorProfileAsync(Guid userId)
  {
    try
    {
      var doctor = await appContext
        .Doctors.Where(d => d.UserId == userId)
        .Include(d => d.User)
        .Include(d => d.DoctorAvailabilities)
        .Include(d => d.DoctorSpecialities)
        .Include(d => d.Educations)
        .Include(d => d.Experiences)
        .SingleOrDefaultAsync();

      if (doctor == null)
      {
        throw new KeyNotFoundException(
          "Doctor with that user id is not found. Couldn't retrieve profile information."
        );
      }

      return doctor.ToDoctorProfileDto(
        doctor.User,
        doctor.DoctorAvailabilities,
        doctor
          .DoctorSpecialities.Where(ds => ds.Speciality != null)
          .Select(ds => ds.Speciality!)
          .ToList(),
        doctor.Educations,
        doctor.Experiences
      );
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured tring to get doctor profile");
      throw;
    }
  }

  public async Task<EducationDto> CreateEducationAsync(
    CreateEducationDto createEducationDto,
    Guid doctorId
  )
  {
    try
    {
      var education = await appContext.Educations.AddAsync(
        createEducationDto.ToEducation(doctorId)
      );
      await appContext.SaveChangesAsync();
      return education.Entity.ToEducationDto();
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to create education");
      throw;
    }
  }

  public async Task DeleteAllEducationsAsync(Guid doctorId)
  {
    try
    {
      var educations = appContext.Educations.Where(e => e.DoctorId == doctorId);

      if (!educations.Any())
      {
        throw new KeyNotFoundException($"No educations found for the given doctorId: {doctorId}");
      }

      appContext.Educations.RemoveRange(educations);
      await appContext.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      logger.LogError(
        ex,
        $"An error occurred while trying to delete all educations for doctorId: {doctorId}"
      );
      throw;
    }
  }

  public async Task DeleteAllExperiencesAsync(Guid doctorId)
  {
    try
    {
      var experiences = appContext.Experiences.Where(e => e.DoctorId == doctorId);

      if (!experiences.Any())
      {
        throw new KeyNotFoundException($"No experiences found for the given doctorId: {doctorId}");
      }

      appContext.Experiences.RemoveRange(experiences);
      await appContext.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: AN error occured trying to delete all experiences");
      throw;
    }
  }

  public async Task<ExperienceDto> CreateExperienceAsync(
    CreateExperienceDto createExperienceDto,
    Guid doctorId
  )
  {
    try
    {
      var experience = await appContext.Experiences.AddAsync(
        createExperienceDto.ToExperience(doctorId)
      );
      await appContext.SaveChangesAsync();
      return experience.Entity.ToExperienceDto();
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to create education");
      throw;
    }
  }

  public async Task<List<Experience>> GetDoctorExperiences(Guid doctorId)
  {
    try
    {
      return await appContext.Experiences.Where(e => e.DoctorId == doctorId).ToListAsync();
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured when getting doctor experiences.");
      throw;
    }
  }

  public async Task<List<Education>> GetDoctorEducations(Guid doctorId)
  {
    try
    {
      return await appContext.Educations.Where(e => e.DoctorId == doctorId).ToListAsync();
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: An error occured when getting doctor educations.");
      throw;
    }
  }
}
