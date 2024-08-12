using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;
using HealthHub.Source;
namespace HealthHub.Source.Services;

/// <summary>
/// User Service
/// </summary>
/// <param name="appContext"></param>
/// <param name="auth0Service"></param>
/// <param name="logger"></param>
public class UserService(Data.AppContext appContext, Auth0Service auth0Service, ILogger<UserService> logger)
{

  /// <summary>
  /// Check if Email is Verified by Auth0
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  public async Task<ServiceResponse<bool?>> CheckEmailVerified(Guid userId)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

      if (user == null)
      {
        return new ServiceResponse<bool?>(
          false,
          404,
          false,
          "User with that id not found"
        );
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
        return new ServiceResponse<bool?>(
          false,
          400,
          false,
          "User does not have an Auth0Id"
        );
      }

      var isEmailVerified = await auth0Service.IsEmailVerified(user.Auth0Id);

      if (isEmailVerified == null)
      {
        return new ServiceResponse<bool?>(
          false,
          500,
          false,
          "Failed to check email verification"
        );
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

      return new ServiceResponse<bool?>(
        false,
        500,
        false,
        "An error occured trying to check email verification."
      );
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
      return new ServiceResponse<List<UserDto>>(
        Success: false,
        StatusCode: 500,
        Message: "Failed to retrieve users",
        Data: null
      );
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
        return new ServiceResponse<UserDto>(
          false,
          400,
          null,
          "User with that email already exists!."
        );
      }

      // Search the User By Phone
      var userByPhone = await appContext.Users.AnyAsync(u => u.Phone == registerUserDto.Phone);

      // If User is Found With That Phone
      if (userByPhone)
      {
        return new ServiceResponse<UserDto>(
          false,
          400,
          null,
          "User with that phone already exists!."
        );
      }

      // Create User in Auth0
      auth0User = await auth0Service.CreateUserAsync(registerUserDto);

      logger.LogInformation($"Auth0Created User in UserService \n\n:{auth0User}");


      if (auth0User == null || auth0User.UserId == null)
      {
        return new ServiceResponse<UserDto>(
          false,
          500,
          null,
          "Failed to create user in Auth0"
        );
      }

      // Convert the Dto to User Entity 
      var user = registerUserDto.ToEntity();

      // Populate the User Entity with the Auth0User Accordingly
      user.Auth0Id = auth0User.UserId;
      user.ProfilePicture = auth0User.Profile;
      user.IsEmailVerified = auth0User.EmailVerified;

      // Add the User to the Database 
      var addedUser = await appContext.Users.AddAsync(user);

      // COMMENTED INTENTIONALLY BECAUSE AUTH0 PROVIDES CUSTOM EMAIL VERIFICATION
      // Send Otp to user if their email isn't verified
      // if (!auth0User.EmailVerified)
      //   await authService.SendOtp(addedUser.Entity.UserId);

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

      return new ServiceResponse<UserDto>(
        Success: false,
        StatusCode: 500,
        null,
        "Failed to register user"
      );
    }
  }


  public async Task<ServiceResponse> DeleteUserAsync(Guid userId)
  {
    try
    {
      var user = await appContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

      if (user == null)
      {
        return new ServiceResponse(
          false,
          404,
          "User not found"
        );
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

      return new ServiceResponse(
        false,
        500,
        "An error occured trying to delete user."
      );
    }
  }
}