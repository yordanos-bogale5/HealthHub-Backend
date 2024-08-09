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
      User user = registerUserDto.ToEntity();
      await appContext.Users.AddAsync(user);
      await appContext.SaveChangesAsync();
      return new ServiceResponse<Guid>(
        Success: true,
        StatusCode: 201,
        Message: "Registration Success!",
        Data: user.UserId
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw new Exception("Registration Failed", ex);
    }
  }
}