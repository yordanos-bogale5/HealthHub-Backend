using HealthHub.Source.Dtos;

namespace HealthHub.Source.Extensions;

public static class DtoExtensions
{
  public static User ToEntity(this RegisterUserDto userDto)
  {
    return new User()
    {
      FirstName = userDto.FirstName,
      LastName = userDto.LastName,
      Email = userDto.Email,
      Password = userDto.Password,
      Phone = userDto.Phone,
      Address = userDto.Address
    };
  }
}