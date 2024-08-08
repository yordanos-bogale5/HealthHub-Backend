using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Services;

public class UserService(AppContext appContext)
{
  public JsonResult GetAllUsers()
  {
    return new JsonResult(appContext.Users.ToList());
  }

  public JsonResult RegisterUser()
  {
    return new JsonResult("Registered");
  }
}