using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Source.Services;

public class UserService(AppContext appContext)
{
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

      // Convert the Dto to Entity 
      var user = registerUserDto.ToEntity();
      // Add the User to the Database Asyncronously
      var addedUser = await appContext.Users.AddAsync(user);
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