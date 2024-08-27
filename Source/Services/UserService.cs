using System.ComponentModel;
using System.Text.Json;
using HealthHub.Source;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace HealthHub.Source.Services;

/// <summary>
/// User Service
/// </summary>
/// <param name="appContext"></param>
/// <param name="auth0Service"></param>
/// <param name="logger"></param>
/// <param name="patientService"></param>
/// <param name="doctorService"></param>
/// <param name="adminService"></param>
/// <param name="specialityService"></param>
/// <param name="doctorSpecialityService"></param>
public class UserService(
  ApplicationContext appContext,
  Auth0Service auth0Service,
  ILogger<UserService> logger,
  PatientService patientService,
  DoctorService doctorService,
  AvailabilityService availabilityService,
  AdminService adminService,
  SpecialityService specialityService,
  DoctorSpecialityService doctorSpecialityService,
  AppointmentService appointmentService
)
{
  /// <summary>
  /// Check if Email is Verified by Auth0
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  public async Task<ServiceResponse<bool?>> CheckEmailVerified(string email)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.Email == email);

      if (user == null)
      {
        throw new KeyNotFoundException("User with that email is not found");
      }

      if (user.IsEmailVerified)
      {
        return new ServiceResponse<bool?>(true, 200, true, "Email is verified");
      }

      if (user.Auth0Id == null)
      {
        throw new KeyNotFoundException("User doesn't have an account.");
      }

      var isEmailVerified = await auth0Service.IsEmailVerified(user.Auth0Id);

      if (isEmailVerified == null)
      {
        throw new Exception("Failed to verify email.");
      }

      // Sync the auth0 email verification status with the user entity
      user.IsEmailVerified = (bool)isEmailVerified;

      await appContext.SaveChangesAsync();

      return new ServiceResponse<bool?>(
        true,
        200,
        isEmailVerified,
        (bool)isEmailVerified ? "Email is verified" : "Email is not verified"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to check email verification");

      throw new Exception("Failed to check email verification.");
    }
  }

  /// <summary>
  /// Get All Users
  /// </summary>
  /// <returns>A list of all users</returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<List<UserDto>>> GetAllUsers()
  {
    try
    {
      List<UserDto> users = await appContext.Users.Select(User => User.ToUserDto()).ToListAsync();
      return new ServiceResponse<List<UserDto>>(
        Success: true,
        StatusCode: 200,
        Message: "Users Retrieved",
        Data: users
      );
    }
    catch (Exception ex)
    {
      Console.Write(ex);
      throw new Exception("Failed to retrieve users.");
    }
  }

  /// <summary>
  /// Register User Service
  /// </summary>
  /// <param name="registerUserDto"></param>
  /// <returns>The Guid of the newly created user.</returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<UserDto>> RegisterUser(RegisterUserDto registerUserDto)
  {
    Auth0UserDto? auth0User = null;
    try
    {
      // Search the User By Email
      var userByEmail = await appContext.Users.AnyAsync(u => u.Email == registerUserDto.Email);

      // If User is Found By That Email
      if (userByEmail)
      {
        throw new BadHttpRequestException("User with that email already exists!.");
      }

      // Search the User By Phone
      var userByPhone = await appContext.Users.AnyAsync(u => u.Phone == registerUserDto.Phone);

      // If User is Found With That Phone
      if (userByPhone)
      {
        throw new BadHttpRequestException("User with that phone already exists!.");
      }

      Guid userId = Guid.NewGuid();

      // Create User in Auth0
      auth0User = await auth0Service.CreateUserAsync(registerUserDto, userId);

      logger.LogInformation($"Auth0Created User in UserService \n\n:{auth0User}");

      if (auth0User == null || auth0User.UserId == null)
      {
        throw new Exception("Failed to Create user in Auth0");
      }

      // Convert the Dto to User Entity
      var user = registerUserDto.ToUser();

      // Populate the User Entity with the Auth0User Accordingly
      user.UserId = userId;
      user.Auth0Id = auth0User.UserId;
      user.ProfilePicture = auth0User.Profile;
      user.IsEmailVerified = auth0User.EmailVerified;

      // Add the User to the Database
      var addedUser = await appContext.Users.AddAsync(user);

      Role role = registerUserDto.Role.ConvertToEnum<Role>();

      // Create a Patient table for the user
      if (role == Role.Patient)
      {
        await patientService.CreatePatientAsync(
          registerUserDto.ToCreatePatientDto(addedUser.Entity)
        );
      }
      // Create a Doctor table for the user
      else if (role == Role.Doctor)
      {
        // Create the doctor
        var doctor = await doctorService.CreateDoctorAsync(
          registerUserDto.ToCreateDoctorDto(addedUser.Entity)
        );

        if (doctor == null)
        {
          logger.LogError("Error creating Doctor, returned null from doctor service.");
          throw new Exception("Error creating Doctor");
        }

        // Create the specialities
        var specialities = await specialityService.CreateSpecialitiesAsync(
          registerUserDto.Specialities.ToSpecialityList(doctor.DoctorId)
        );

        if (specialities == null)
        {
          logger.LogError("Error creating specialities, returned null from speciality service.");
          throw new Exception("Error creating specialities");
        }

        var createDoctorSpecialityDtos = specialities
          .Select(s => new CreateDoctorSpecialityDto { Doctor = doctor, Speciality = s })
          .ToList();

        var doctorSpecialities = await doctorSpecialityService.CreateDoctorSpecialitiesAsync(
          createDoctorSpecialityDtos
        );

        if (doctorSpecialities == null)
        {
          throw new Exception("Error creating doctor-specialities in service.");
        }

        var availabilities = await availabilityService.AddDoctorAvailabilityAsync(
          registerUserDto.Availabilities,
          doctor
        );

        if (availabilities == null)
        {
          throw new Exception("Error creating availabilities in service.");
        }
      }
      // Create a Admin table for the user
      else
      {
        var admin = adminService.CreateAdminAsync(
          registerUserDto.ToCreateAdminDto(addedUser.Entity)
        );
      }

      // Save the Changes
      await appContext.SaveChangesAsync();

      return new ServiceResponse<UserDto>(
        Success: true,
        StatusCode: 201,
        Message: "Registration Success!",
        Data: addedUser.Entity.ToUserDto()
      );
    }
    catch (Exception ex)
    {
      // Rollback auth0 user creation
      if (auth0User != null)
        await auth0Service.DeleteUserAsync(auth0User.UserId);

      logger.LogError(ex, "Failed to register user");

      throw;
    }
  }

  public async Task<ServiceResponse<Auth0LoginDto>> LoginUserAsync(LoginUserDto loginUserDto)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);

      if (user == null)
      {
        throw new KeyNotFoundException("User with that email is not found");
      }

      if (user.Auth0Id == null)
      {
        throw new KeyNotFoundException("Auth0Id is not found for user.");
      }

      var auth0LoginDto = await auth0Service.LoginUserAsync(loginUserDto, user.Auth0Id);

      logger.LogInformation($"Auth0 Login Dto: {auth0LoginDto}");

      return new ServiceResponse<Auth0LoginDto>(true, 200, auth0LoginDto, "Login success!");
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to login user");
      throw;
    }
  }

  /// <summary>
  /// Delete User Service
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  public async Task<ServiceResponse> DeleteUserAsync(Guid userId)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

      if (user == null)
      {
        throw new KeyNotFoundException("User Not Found!");
      }

      // Delete the User in Auth0
      if (user.Auth0Id != null)
        await auth0Service.DeleteUserAsync(user.Auth0Id);

      // Remove user appointments if exist
      await appointmentService.DeleteAppointmentWhereUserId(userId);

      appContext.Users.Remove(user);
      await appContext.SaveChangesAsync();

      return new ServiceResponse(true, 204, "User Deleted");
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to delete user");
      throw;
    }
  }

  public async Task<ServiceResponse<ProfileDto>> GetUserProfile(Guid userId)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

      if (user == null)
      {
        throw new KeyNotFoundException("User not found.");
      }

      return new ServiceResponse<ProfileDto>(
        true,
        200,
        user.ToProfileDto(),
        "User Profile Retrieved"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to retrieve user");

      throw;
    }
  }

  // public async Task<ProfileDto>
}
