using System.Text.Json;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

public class DoctorService(ApplicationContext appContext, ILogger<DoctorService> logger)
{
    public async Task<Doctor?> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
    {
        try
        {
            var doctorResult = await appContext.Doctors.AddAsync(createDoctorDto.ToDoctor());
            var doctor = doctorResult.Entity;
            logger.LogInformation($"\nCreate Doctor Result: \n {doctorResult}");
            await appContext.SaveChangesAsync();
            return doctor;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to Create Doctor");
            throw;
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
            var doctorUsers = await appContext
                .Doctors.Include(d => d.User) // Ensure the related User entity is loaded
                .Include(d => d.DoctorSpecialities)
                .ThenInclude(ds => ds.Speciality)
                .Select(d => new DoctorUser
                {
                    FirstName = d.User.FirstName,
                    LastName = d.User.LastName,
                    Email = d.User.Email,
                    Phone = d.User.Phone,
                    Gender = d.User.Gender,
                    DateOfBirth = d.User.DateOfBirth,
                    Address = d.User.Address,
                    Specialities = d
                        .DoctorSpecialities.Select(ds => ds.Speciality.SpecialityName)
                        .ToList(),
                    Qualifications = d.Qualifications,
                    Biography = d.Biography,
                    DoctorStatus = d.DoctorStatus
                })
                .ToListAsync();

            return new ServiceResponse<List<DoctorUser>>(
                true,
                200,
                doctorUsers,
                "All Doctors Retrieved!"
            );
        }
        catch (System.Exception ex)
        {
            logger.LogError($"Failed to get all Doctors in User Service: {ex}");
            throw;
        }
    }

    public async Task<ServiceResponse<List<DoctorSpeciality>>> GetDoctorsBySpecialityAsync(
        string specialityName
    )
    {
        try
        {
            var doctors = await appContext
                .DoctorSpecialities.Where(ds =>
                    EF.Functions.Like("SpecialityName", $"%{specialityName}%")
                )
                .Include(ds => ds.Doctor)
                .ThenInclude(d => d.User)
                .Include(ds => ds.Speciality)
                .ToListAsync();
            return new ServiceResponse<List<DoctorSpeciality>>(
                true,
                200,
                doctors,
                "Doctors with speciality name retrieved"
            );
        }
        catch (System.Exception ex)
        {
            logger.LogError($"Failed to get doctors by speciality in doctor service {ex}");
            throw;
        }
    }

    // public async Task<ServiceResponse<List<DoctorUser>>> GetDoctorsByNameAsync(string doctorName)
    // {
    //   try
    //   {
    //     var response = await SearchDoctor("FirstName", doctorName);
    //     if (!response.Success) throw new Exception(response.Message);
    //     return new ServiceResponse<List<DoctorUser>>(true, 200, response.Data, "Doctors with name retrieved");
    //   }
    //   catch (System.Exception ex)
    //   {
    //     throw new Exception("Failed to get doctors by name in doctor service.");
    //   }
    // }
}
