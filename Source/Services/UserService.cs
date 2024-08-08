using HealthHub.Source.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Services;

public class UserService(AppContext appContext)
{
  public JsonResult GetAllUsers()
  {
    return new JsonResult(appContext.Users.ToList());
  }

  public JsonResult RegisterUser(RegisterUserDto registerUserDto)
  {
    return new JsonResult("Registered");
  }
}