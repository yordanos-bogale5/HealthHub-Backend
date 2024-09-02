using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace HealthHub.Source.Helpers.Extensions;

public static class DtoExtensions
{
  /// <summary>
  /// Maps RegisterUserDto to User, Notice: You will lose password property from returned User though!
  /// </summary>
  /// <param name="userDto"></param>
  /// <returns>User instance</returns>
  public static User ToUser(this RegisterUserDto userDto)
  {
    return new User()
    {
      FirstName = userDto.FirstName,
      LastName = userDto.LastName,
      Email = userDto.Email,
      Phone = userDto.Phone,
      Address = userDto.Address,
      Gender = userDto.Gender.ConvertToEnum<Gender>(),
      Role = userDto.Role.ConvertToEnum<Role>(),
      DateOfBirth = userDto.DateOfBirth.ConvertTo<DateTime>()
    };
  }

  /// <summary>
  /// Maps CreatePatientDto to Patient
  /// </summary>
  /// <param name="createPatientDto"></param>
  /// <returns></returns>
  public static Patient ToPatient(this CreatePatientDto createPatientDto)
  {
    return new Patient()
    {
      User = createPatientDto.User,
      UserId = createPatientDto.User.UserId,
      MedicalHistory = createPatientDto.MedicalHistory,
      EmergencyContactName = createPatientDto.EmergencyContactName,
      EmergencyContactPhone = createPatientDto.EmergencyContactPhone
    };
  }

  /// <summary>
  /// Maps CreateDoctorDto to Doctor
  /// </summary>
  /// <param name="createDoctorDto"></param>
  /// <returns></returns>
  public static Doctor ToDoctor(this CreateDoctorDto createDoctorDto)
  {
    return new Doctor()
    {
      User = createDoctorDto.User,
      UserId = createDoctorDto.User.UserId,
      Qualifications = createDoctorDto.Qualifications,
      Biography = createDoctorDto.Biography,
      DoctorStatus = createDoctorDto.DoctorStatus
    };
  }

  /// <summary>
  /// Maps CreateAdminDto to Admin
  /// </summary>
  /// <param name="createAdminDto"></param>
  /// <returns></returns>
  public static Admin ToAdmin(this CreateAdminDto createAdminDto)
  {
    return new Admin() { User = createAdminDto.User, UserId = createAdminDto.User.UserId };
  }

  public static Speciality ToSpeciality(this CreateSpecialityDto specialityDto)
  {
    return new Speciality() { SpecialityName = specialityDto.SpecialityName };
  }

  public static DoctorSpeciality ToDoctorSpeciality(
    this CreateDoctorSpecialityDto createDoctorSpecialityDto,
    Guid doctorId,
    Guid specialityId
  )
  {
    return new DoctorSpeciality() { DoctorId = doctorId, SpecialityId = specialityId };
  }
}
