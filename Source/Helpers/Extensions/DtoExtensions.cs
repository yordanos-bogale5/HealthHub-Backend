using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;


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
      Gender = userDto.Gender,
      Role = userDto.Role
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
      UserId = createPatientDto.UserId,
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
      UserId = createDoctorDto.UserId,
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
    return new Admin()
    {
      UserId = createAdminDto.UserId
      // More to be added here if admin changes in future, currently only UserId is needed
    };
  }


  public static Speciality ToSpeciality(this CreateSpecialityDto specialityDto)
  {
    return new Speciality()
    {
      DoctorId = specialityDto.DoctorId,
      SpecialityName = specialityDto.SpecialityName
    };
  }
}