using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;

public class DoctorService(ApplicationContext appContext, ILogger<DoctorService> logger)
{
  public async Task<Doctor?> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
  {
    try
    {
      var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
      var doctor = doctorResult.Entity;

      await appContext.SaveChangesAsync();
      return doctor;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Doctor");
      throw new Exception("Failed to Create Doctor");
    }
  }

  public async Task<Doctor?> GetDoctor(Guid userId)
  {
    try
    {
      var doctor = await appContext.Doctors.SingleOrDefaultAsync(d => d.UserId == userId);
      return doctor;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured when trying to get doctor by id {ex}");
      throw;
    }
  }

  public async Task<ServiceResponse<List<DoctorUser>>> GetAllDoctors()
  {
    try
    {
      List<DoctorUser> doctors = await appContext.Specialities
        .Include(s => s.Doctor)
            .ThenInclude(d => d.User)
        .Select(s => new DoctorUser
        {
          Address = s.Doctor!.User!.Address,
          DateOfBirth = s.Doctor.User.DateOfBirth,
          Email = s.Doctor.User.Email,
          FirstName = s.Doctor.User.FirstName,
          Gender = s.Doctor.User.Gender,
          LastName = s.Doctor.User.LastName,
          Phone = s.Doctor.User.Phone,
          Biography = s.Doctor.Biography,
          DoctorStatus = s.Doctor.DoctorStatus,
          Qualifications = s.Doctor.Qualifications,
          Specialities = appContext.Specialities
                .Where(spec => spec.DoctorId == s.DoctorId)
                .Select(spec => spec.SpecialityName)
                .ToList()
        })
        .ToListAsync();


      var jsonDoc = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
      Console.WriteLine($"\n\n{jsonDoc}\n\n");


      return new ServiceResponse<List<DoctorUser>>(true, 200, doctors, "All Doctors Retrieved!");
    }
    catch (System.Exception ex)
    {
      throw new Exception("Failed to Get all Doctors in User Service");
    }
  }

  public async Task<ServiceResponse<List<DoctorUser>>> GetDoctorsBySpecialityAsync(string specialityName)
  {
    try
    {
      var doctors = await appContext.Specialities
        .Where(s => EF.Functions.Like(s.SpecialityName, $"%{specialityName}%"))
        .Include(s => s.Doctor)
          .ThenInclude(d => d.User)
        .Select(s => new DoctorUser
        {
          Address = s.Doctor!.User!.Address,
          DateOfBirth = s.Doctor.User.DateOfBirth,
          Email = s.Doctor.User.Email,
          FirstName = s.Doctor.User.FirstName,
          Gender = s.Doctor.User.Gender,
          LastName = s.Doctor.User.LastName,
          Phone = s.Doctor.User.Phone,
          Biography = s.Doctor.Biography,
          DoctorStatus = s.Doctor.DoctorStatus,
          Qualifications = s.Doctor.Qualifications,
          Specialities = appContext.Specialities
                .Where(spec => spec.DoctorId == s.DoctorId)
                .Select(spec => spec.SpecialityName)
                .ToList()
        })
        .ToListAsync();

      return new ServiceResponse<List<DoctorUser>>(true, 200, doctors, "Doctors with speciality name retrieved");
    }
    catch (System.Exception ex)
    {
      throw new Exception("Failed to get doctors by speciality in doctor service.");
    }
  }

}
