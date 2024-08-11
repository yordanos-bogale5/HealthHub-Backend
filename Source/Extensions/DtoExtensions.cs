using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;


namespace HealthHub.Source.Extensions;

public static class DtoExtensions
{
  /// <summary>
  /// Maps RegisterUserDto to User, Notice: You will lose password property from returned User though!
  /// </summary>
  /// <param name="userDto"></param>
  /// <returns>User instance</returns>
  public static User ToEntity(this RegisterUserDto userDto)
  {
    return new User()
    {
      FirstName = userDto.FirstName,
      LastName = userDto.LastName,
      Email = userDto.Email,
      Phone = userDto.Phone,
      Address = userDto.Address
    };
  }
}