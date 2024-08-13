using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Extensions
{
  public static class EntityExtensions
  {
    // Maps User entity to UserDto
    public static UserDto ToUserDto(this User user)
    {
      return new UserDto(
          user.UserId,
          user.FirstName,
          user.LastName,
          user.Email,
          user.ProfilePicture,
          user.Gender
      );
    }

    public static ProfileDto ToProfileDto(this User user)
    {
      return new ProfileDto(
          user.UserId,
          user.FirstName,
          user.LastName,
          user.Email,
          user.ProfilePicture,
          user.Phone,
          user.Gender,
          user.DateOfBirth,
          user.Address,
          user.Role
      );
    }

    // Maps User entity to RegisterUserDto
    public static RegisterUserDto ToRegisterUserDto(this User user, string password)
    {
      return new RegisterUserDto(
          user.FirstName,
          user.LastName,
          user.Email,
          password,
          user.Phone,
          user.Gender,
          user.DateOfBirth,
          user.ProfilePicture,
          user.Address
      );
    }
  }
}
