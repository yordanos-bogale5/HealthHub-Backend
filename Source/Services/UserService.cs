using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Helpers.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;
using HealthHub.Source;
using HealthHub.Source.Data;
using HealthHub.Source.Models.Enums;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
public class UserService(ApplicationContext appContext, Auth0Service auth0Service, ILogger<UserService> logger, PatientService patientService, DoctorService doctorService, AdminService adminService, SpecialityService specialityService)
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
        return new ServiceResponse<bool?>(
          true,
          200,
          true,
          "Email is verified"
        );
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

      // Create User in Auth0
      auth0User = await auth0Service.CreateUserAsync(registerUserDto);

      logger.LogInformation($"Auth0Created User in UserService \n\n:{auth0User}");


      if (auth0User == null || auth0User.UserId == null)
      {
        throw new Exception("Failed to Create user in Auth0");
      }

      // Convert the Dto to User Entity 
      var user = registerUserDto.ToUser();

      // Populate the User Entity with the Auth0User Accordingly
      user.Auth0Id = auth0User.UserId;
      user.ProfilePicture = auth0User.Profile;
      user.IsEmailVerified = auth0User.EmailVerified;


      // Add the User to the Database 
      var addedUser = await appContext.Users.AddAsync(user);


      // Create a Patient table for the user
      if (registerUserDto.Role == Role.Patient)
      {
        await patientService.CreatePatientAsync(new CreatePatientDto
        {
          UserId = addedUser.Entity.UserId,
          MedicalHistory = registerUserDto.MedicalHistory,
          EmergencyContactName = registerUserDto.EmergencyContactName,
          EmergencyContactPhone = registerUserDto.EmergencyContactPhone
        });
      }
      // Create a Doctor table for the user
      else if (registerUserDto.Role == Role.Doctor)
      {


        var doctor = await doctorService.CreateDoctorAsync(new CreateDoctorDto
        {
          UserId = addedUser.Entity.UserId,
          Biography = registerUserDto.Biography!,
          Qualifications = registerUserDto.Qualifications!,
        });

        if (doctor == null)
        {
          throw new Exception("Error creating Doctor");
        }

        var speciality = await specialityService.CreateSpecialitiesAsync(registerUserDto.Specialities.ToSpecialityList(doctor.DoctorId));
      }
      // Create a Admin table for the user
      else
      {
        var admin = adminService.CreateAdminAsync(new CreateAdminDto
        {
          UserId = addedUser.Entity.UserId
        });
      }


      // Save the Changes
      await appContext.SaveChangesAsync();

      return new ServiceResponse<UserDto>(
        Success: true,
        StatusCode: 201,
        Message: "Registration Success!",
        Data: user.ToUserDto()
      );
    }
    catch (Exception ex)
    {
      // Rollback auth0 user creation
      if (auth0User != null)
        await auth0Service.DeleteUserAsync(auth0User.UserId);

      logger.LogError(ex, "Failed to register user");

      throw new Exception("Failed to register user.");
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

      var auth0LoginDto = await auth0Service.LoginUserAsync(loginUserDto);

      logger.LogInformation($"Auth0 Login Dto: {auth0LoginDto}");

      return new ServiceResponse<Auth0LoginDto>(
        true,
        200,
        auth0LoginDto,
        "Login success!"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to login user");
      throw new Exception("Failed to login user.");
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

      appContext.Users.Remove(user);

      await appContext.SaveChangesAsync();

      return new ServiceResponse(
        true,
        204,
        "User Deleted"
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Failed to delete user");

      throw new Exception("Failed to delete user.");
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

      throw new Exception("Problem occured when getting profile.");
    }
  }



}