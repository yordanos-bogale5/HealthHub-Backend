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
/// <param name="authService"></param>
/// <param name="auth0Service"></param>
public class UserService(Data.AppContext appContext, AuthService authService, Auth0Service auth0Service)
{
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
      throw new Exception("Failed to retrieve users", ex);
    }
  }

  /// <summary>
  /// Register User Service
  /// </summary>
  /// <param name="registerUserDto"></param>
  /// <returns>The Guid of the newly created user.</returns>
  /// <exception cref="Exception"></exception>
  public async Task<ServiceResponse<Guid>> RegisterUser(RegisterUserDto registerUserDto)
  {
    try
    {
      // Search the User By Email
      var userByEmail = await appContext.Users.AnyAsync(u => u.Email == registerUserDto.Email);

      // If User is Found By That Email
      if (userByEmail)
      {
        return new ServiceResponse<Guid>(
          false,
          400,
          Guid.Empty,
          "User with that email already exists!."
        );
      }

      // Search the User By Phone
      var userByPhone = await appContext.Users.AnyAsync(u => u.Phone == registerUserDto.Phone);

      // If User is Found With That Phone
      if (userByPhone)
      {
        return new ServiceResponse<Guid>(
          false,
          400,
          Guid.Empty,
          "User with that phone already exists!."
        );
      }

      // Create User in Auth0
      var auth0UserId = await auth0Service.CreateUser(registerUserDto);

      // Convert the Dto to User Entity 
      var user = registerUserDto.ToEntity();

      // Modify the Entity Id to match the Auth0 id
      user.UserId = auth0UserId;

      // Add the User to the Database Asyncronously
      var addedUser = await appContext.Users.AddAsync(user);

      // Send Otp to user
      await authService.SendOtp(addedUser.Entity.UserId);

      // Save the Changes
      await appContext.SaveChangesAsync();

      return new ServiceResponse<Guid>(
        Success: true,
        StatusCode: 201,
        Message: "Registration Success!",
        Data: addedUser.Entity.UserId // Return the userId if needed
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw new Exception("Registration Failed", ex);
    }
  }
}