using HealthHub.Source.Dtos;

namespace HealthHub.Source.Extensions;


public static class EntityExtensions
{
  public static RegisterUserDto ToDto(this User user)
  {
    return new RegisterUserDto(
      user.FirstName,
      user.LastName,
      user.Email,
      user.Password,
      user.Phone,
      user.Gender,
      user.DateOfBirth,
      user.ProfilePicture,
      user.Address
    );
  }
}