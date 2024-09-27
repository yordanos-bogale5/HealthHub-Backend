using System.ComponentModel;
using System.Text.Json;
using AutoMapper;
using HealthHub.Source;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers;
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
/// <param name="appointmentService"></param>
public class UserService(
  ApplicationContext appContext,
  Auth0Service auth0Service,
  ILogger<UserService> logger,
  PatientService patientService,
  DoctorService doctorService,
  AdminService adminService,
  SpecialityService specialityService,
  DoctorSpecialityService doctorSpecialityService,
  AppointmentService appointmentService,
  FileService fileService
)
{
  /// <summary>
  /// Register User Service
  /// </summary>
  /// <param name="registerUserDto"></param>
  /// <returns>The Guid of the newly created user.</returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<ProfileDto>> RegisterUser(RegisterUserDto registerUserDto)
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

      ProfileDto? userProfile = null;

      if (role == Role.Patient)
      {
        Patient patient = await patientService.CreatePatientAsync(
          registerUserDto.ToCreatePatientDto(addedUser.Entity)
        );
        userProfile = addedUser.Entity.ToPatientProfileDto(patient);
      }
      else if (role == Role.Doctor)
      {
        // Create the CV file
        var cvFile = await fileService.CreateFileAsync(
          new CreateFileDto(
            registerUserDto.Cv!.MimeType,
            registerUserDto.Cv!.FileDataBase64,
            registerUserDto.Cv!.FileName
          )
        );

        // Create the doctor
        Doctor doctor = await doctorService.CreateDoctorAsync(
          registerUserDto.ToCreateDoctorDto(
            addedUser.Entity,
            cvFile,
            registerUserDto.Educations,
            registerUserDto.Experiences
          )
        );

        // Create the specialities
        var specialities = await specialityService.CreateSpecialitiesAsync(
          registerUserDto.Specialities.ToSpecialityList(doctor.DoctorId)
        );

        var createDoctorSpecialityDtos = specialities
          .Select(s => new CreateDoctorSpecialityDto
          {
            DoctorId = doctor.DoctorId,
            SpecialityId = s.SpecialityId
          })
          .ToList();

        // Create the speciality-doctor association
        var doctorSpecialities = await doctorSpecialityService.CreateDoctorSpecialitiesAsync(
          createDoctorSpecialityDtos
        );

        // Create the availabilities
        var availabilities = await doctorService.AddDoctorAvailabilityAsync(
          registerUserDto.Availabilities,
          doctor
        );

        ICollection<Education> educations = await doctorService.GetDoctorEducations(
          doctor.DoctorId
        );
        ICollection<Experience> experiences = await doctorService.GetDoctorExperiences(
          doctor.DoctorId
        );

        userProfile = addedUser.Entity.ToDoctorProfileDto(
          doctor,
          availabilities,
          specialities,
          educations,
          experiences
        );
      }
      // Create a Admin table for the user
      else
      {
        var admin = adminService.CreateAdminAsync(
          registerUserDto.ToCreateAdminDto(addedUser.Entity)
        );
        userProfile = addedUser.Entity.ToProfileDto();
      }

      // Save the Changes
      await appContext.SaveChangesAsync();

      return new ServiceResponse<ProfileDto>(
        Success: true,
        StatusCode: 201,
        Message: "Registration Success! We have sent you an email verification link to your email. Please verify your account.",
        Data: userProfile
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
    catch (Exception ex)
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
      await appointmentService.DeleteAppointmentWhereUserIdAsync(userId);

      appContext.Users.Remove(user);
      await appContext.SaveChangesAsync();

      return new ServiceResponse(true, 204, "User Deleted");
    }
    catch (Exception ex)
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

      ProfileDto? profileDto = null;

      if (user.Role == Role.Patient)
      {
        profileDto = await patientService.GetPatientProfileAsync(userId);
      }
      else if (user.Role == Role.Doctor)
      {
        profileDto = await doctorService.GetDoctorProfileAsync(userId);
      }

      return new ServiceResponse<ProfileDto>(true, 200, profileDto, "User Profile Retrieved");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to retrieve user");

      throw;
    }
  }

  /// <summary>
  /// Returns the edited profile of a user. Whether be it a patient or a doctor.
  /// It returns a polymorphic profile dto (doctorProfile or patientProfile) that
  /// depends on the user role associated with the userId
  /// </summary>
  /// <param name="editProfileDto"></param>
  /// <returns>Either returns PatientProfileDto or DoctorProfileDto</returns>
  /// <exception cref="KeyNotFoundException"/>
  public async Task<ServiceResponse<ProfileDto>> EditUserProfileAsync(EditProfileDto editProfileDto)
  {
    try
    {
      Guid userId = editProfileDto.UserId.ToGuid();
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
      if (user == null)
      {
        throw new KeyNotFoundException("User with that id is not found.");
      }

      // Update Firstname
      user.FirstName = editProfileDto.FirstName ?? user.FirstName;

      // Update LastName
      user.LastName = editProfileDto.LastName ?? user.LastName;

      // Update Date of birth
      user.DateOfBirth =
        editProfileDto.DateOfBirth == null
          ? user.DateOfBirth
          : editProfileDto.DateOfBirth.ConvertTo<DateOnly>();

      // Update phone number
      user.Phone = editProfileDto.Phone ?? user.Phone;

      // Update ProfilePicture
      user.ProfilePicture = editProfileDto.ProfilePicture ?? user.ProfilePicture;

      // Update Gender
      user.Gender =
        editProfileDto.Gender != null ? editProfileDto.Gender.ConvertToEnum<Gender>() : user.Gender;

      // Update Address
      user.Address = editProfileDto.Address ?? user.Address;

      // If you want to edit the email then make sure to make the current
      // email unverified for prompting user to verify (the new email)
      if (editProfileDto.Email != null && editProfileDto.Email != user.Email)
      {
        // Update the email
        user.Email = editProfileDto.Email;
        user.IsEmailVerified = false;
      }

      // Used to store the return value
      ProfileDto? profileDto = null;

      if (user.Role == Role.Patient)
      {
        // Edit the Patient Profile and store to profileDto
        profileDto = await patientService.EditPatientProfileAsync(
          new EditPatientProfileDto(
            userId,
            editProfileDto.MedicalHistory,
            editProfileDto.EmergencyContactName,
            editProfileDto.EmergencyContactPhone
          )
        );
      }
      else if (user.Role == Role.Doctor)
      {
        // Edit Doctor Profile and store to profileDto
        profileDto = await doctorService.EditDoctorProfileAsync(
          new EditDoctorProfileDto(
            userId,
            editProfileDto.Specialities,
            editProfileDto.Qualifications,
            editProfileDto.Biography,
            editProfileDto.Availabilities,
            editProfileDto.DoctorStatus,
            editProfileDto.Educations,
            editProfileDto.Experiences
          )
        );
      }

      await appContext.SaveChangesAsync(); // Save all updates
      return new ServiceResponse<ProfileDto>(true, 200, profileDto, "Profile Update Success.");
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: AN error occured when trying to edit profile information.");
      throw;
    }
  }

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
    catch (Exception ex)
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
  public async Task<ServiceResponse<List<ProfileDto?>>> GetAllUsers()
  {
    try
    {
      var users = await appContext.Users.ToListAsync();

      List<ProfileDto?> profiles = [];

      foreach (User u in users)
      {
        if (u.Role == Role.Patient)
        {
          profiles.Add(await patientService.GetPatientProfileAsync(u.UserId));
        }
        else if (u.Role == Role.Doctor)
        {
          profiles.Add(await doctorService.GetDoctorProfileAsync(u.UserId));
        }
      }

      return new ServiceResponse<List<ProfileDto?>>(
        Success: true,
        StatusCode: 200,
        Message: "Users Retrieved",
        Data: profiles
      );
    }
    catch (Exception ex)
    {
      Console.Write(ex);
      throw;
    }
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="userId"></param>
  /// <returns>True if the user exists, otherwise False</returns>
  public async Task<bool> UserExistsAsync(Guid userId)
  {
    try
    {
      var result = await appContext.Users.FindAsync(userId);
      return result != null;
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }
}
