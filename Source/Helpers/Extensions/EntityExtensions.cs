using HealthHub.Source.Helpers;
using HealthHub.Source.Middlewares;
using HealthHub.Source.Models.Defaults;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Extensions;

public static class EntityExtensions
{
  // Maps User entity to UserDto
  public static UserDto ToUserDto(this User user)
  {
    return new UserDto
    {
      UserId = user.UserId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      ProfilePicture = user.ProfilePicture ?? "",
      Gender = user.Gender,
      Address = user.Address,
      DateOfBirth = user.DateOfBirth,
      Phone = user.Phone,
    };
  }

  public static PatientProfileDto ToPatientProfileDto(this User user, Patient patient)
  {
    return new PatientProfileDto
    {
      UserId = user.UserId,
      PatientId = patient.PatientId,
      Address = user.Address,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Role = user.Role,
      EmergencyContactName = patient.EmergencyContactName ?? "",
      EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
      MedicalHistory = patient.MedicalHistory ?? ""
    };
  }

  public static DoctorProfileDto ToDoctorProfileDto(
    this User user,
    Doctor doctor,
    ICollection<DoctorAvailability> doctorAvailability,
    ICollection<Speciality> specialities,
    ICollection<Education> educations,
    ICollection<Experience> experiences
  )
  {
    return new DoctorProfileDto
    {
      UserId = user.UserId,
      DoctorId = doctor.DoctorId,
      Address = user.Address,
      Availabilities = doctorAvailability.Select(da => da.ToAvailabilityDto()).ToList(),
      Biography = doctor.Biography,
      DateOfBirth = user.DateOfBirth,
      DoctorStatus = doctor.DoctorStatus,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Qualifications = doctor.Qualifications,
      Role = user.Role,
      Specialities = specialities.Select(s => s.ToSpecialityDto()).ToList(),
      Educations = educations.Select(e => e.ToEducationDto()).ToList(),
      Experiences = experiences.Select(e => e.ToExperienceDto()).ToList(),
    };
  }

  public static CreatePatientDto ToCreatePatientDto(this RegisterUserDto registerUserDto, User user)
  {
    return new CreatePatientDto
    {
      User = user,
      EmergencyContactName = registerUserDto.EmergencyContactName,
      EmergencyContactPhone = registerUserDto.EmergencyContactPhone,
      MedicalHistory = registerUserDto.MedicalHistory
    };
  }

  public static CreateDoctorDto ToCreateDoctorDto(
    this RegisterUserDto registerUserDto,
    User user,
    HealthHub.Source.Models.Entities.File cv,
    List<CreateEducationDto> createEducationDtos,
    List<CreateExperienceDto> createExperienceDtos
  )
  {
    return new CreateDoctorDto
    {
      Biography = registerUserDto.Biography ?? "None",
      Qualifications = registerUserDto.Qualifications ?? "None",
      User = user,
      Cv = cv,
      Educations = createEducationDtos,
      Experiences = createExperienceDtos
    };
  }

  public static ProfileDto ToProfileDto(this User user)
  {
    return new ProfileDto
    {
      UserId = user.UserId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      ProfilePicture = user.ProfilePicture ?? "",
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      Role = user.Role
    };
  }

  // Maps User entity to RegisterUserDto
  public static RegisterUserDto ToRegisterUserDto(this User user, string password)
  {
    return new RegisterUserDto
    {
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      Password = password,
      Phone = user.Phone,
      Gender = user.Gender.ToString(),
      Role = user.Role.ToString(),
      DateOfBirth = user.DateOfBirth.ToString(),
      Address = user.Address
    };
  }

  public static CreateAdminDto ToCreateAdminDto(this RegisterUserDto registerUserDto, User user)
  {
    return new CreateAdminDto { User = user };
  }

  /// <summary>
  /// Make sure to populate User and Doctor Specialities Before calling this Extension Method
  /// </summary>
  /// <param name="d"></param>
  /// <param name="user"></param>
  /// <param name="doctorSpecialities"></param>
  /// <returns></returns>
  public static DoctorDto ToDoctorDto(
    this Doctor d,
    User user,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new DoctorDto
    {
      UserId = user.UserId,
      DoctorId = d.DoctorId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      IsEmailVerified = user.IsEmailVerified,
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      Specialities = doctorSpecialities.Select(ds => ds.Speciality.SpecialityName).ToList(),
      Qualifications = d.Qualifications,
      Biography = d.Biography,
      DoctorStatus = d.DoctorStatus,
      ProfilePicture = user.ProfilePicture ?? ""
    };
  }

  public static PatientDto ToPatientDto(this Patient patient, User user)
  {
    return new PatientDto
    {
      UserId = user.UserId,
      PatientId = patient.PatientId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      IsEmailVerified = user.IsEmailVerified,
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      EmergencyContactName = patient.EmergencyContactName ?? "",
      EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
      MedicalHistory = patient.MedicalHistory ?? "",
      ProfilePicture = patient.User.ProfilePicture ?? "",
    };
  }

  /// <summary>
  /// Maps Appointment entity to AppointmentDto
  /// </summary>
  /// <param name="appointment">appointment</param>
  /// <param name="doctor"></param>
  /// <param name="patient"></param>
  /// <param name="doctorUser"></param>
  /// <param name="patientUser"></param>
  /// <param name="user"></param>
  /// <param name="doctorSpecialities"></param>
  /// <returns></returns>
  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Doctor doctor,
    Patient patient,
    User doctorUser,
    User patientUser,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Doctor = doctor.ToDoctorDto(doctorUser, doctorSpecialities),
      Patient = patient.ToPatientDto(patientUser),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Patient patient,
    User patientUser
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Patient = patient.ToPatientDto(patientUser),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Doctor doctor,
    User doctorUser,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Doctor = doctor.ToDoctorDto(doctorUser, doctorSpecialities),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static CreateDoctorSpecialityDto ToCreateDoctorSpecialityDto(
    this Speciality speciality,
    Doctor doctor
  )
  {
    return new CreateDoctorSpecialityDto
    {
      DoctorId = doctor.DoctorId,
      SpecialityId = speciality.SpecialityId
    };
  }

  public static DoctorProfileDto ToDoctorProfileDto(
    this Doctor doctor,
    User user,
    ICollection<DoctorAvailability> doctorAvailability,
    ICollection<Speciality> specialities,
    ICollection<Education> educations,
    ICollection<Experience> experiences
  )
  {
    return new DoctorProfileDto
    {
      UserId = user.UserId,
      DoctorId = doctor.DoctorId,
      Address = user.Address,
      Availabilities = doctorAvailability.Select(da => da.ToAvailabilityDto()).ToList(),
      Biography = doctor.Biography,
      DateOfBirth = user.DateOfBirth,
      DoctorStatus = doctor.DoctorStatus,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Qualifications = doctor.Qualifications,
      Role = user.Role,
      Specialities = specialities.Select(s => s.ToSpecialityDto()).ToList(),
      Educations = educations.Select(e => e.ToEducationDto()).ToList(),
      Experiences = experiences.Select(e => e.ToExperienceDto()).ToList(),
    };
  }

  public static PatientProfileDto ToPatientProfileDto(this Patient patient, User user)
  {
    return new PatientProfileDto
    {
      UserId = user.UserId,
      PatientId = patient.PatientId,
      Address = user.Address,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Role = user.Role,
      EmergencyContactName = patient.EmergencyContactName ?? "",
      EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
      MedicalHistory = patient.MedicalHistory ?? ""
    };
  }

  public static AvailabilityDto ToAvailabilityDto(this DoctorAvailability doctorAvailability)
  {
    return new AvailabilityDto(
      doctorAvailability.AvailableDay.GetDisplayName(),
      doctorAvailability.StartTime.ToString(),
      doctorAvailability.EndTime.ToString()
    );
  }

  public static string ToSpecialityDto(this Speciality speciality)
  {
    return speciality.SpecialityName;
  }

  public static EducationDto ToEducationDto(this Education education)
  {
    return new EducationDto(
      education.EducationId,
      education.Degree,
      education.Institution,
      education.StartDate,
      education.EndDate,
      education.DoctorId
    );
  }

  public static ExperienceDto ToExperienceDto(this Experience experience)
  {
    return new ExperienceDto(
      experience.ExperienceId,
      experience.Institution,
      experience.StartDate,
      experience.EndDate,
      experience.Description,
      experience.DoctorId
    );
  }

  public static MessageDto ToMessageDto(
    this Message message,
    ICollection<HealthHub.Source.Models.Entities.File>? files
  )
  {
    return new MessageDto(
      message.SenderId,
      message.ReceiverId,
      message.MessageText,
      files != null ? files.Select(f => f.ToFileDto()).ToList() : []
    );
  }

  public static FileDto ToFileDto(this HealthHub.Source.Models.Entities.File file)
  {
    return new FileDto(
      file.FileId,
      Mime.GetReverseMime(file.MimeType),
      FileHelper.ToBase64(file.FileData),
      file.FileName,
      file.FileSize
    );
  }

  public static ConversationDto ToConversationDto(
    this Conversation conversation,
    ICollection<Message> messages,
    ICollection<HealthHub.Source.Models.Entities.File>? files
  )
  {
    return new ConversationDto(
      conversation.ConversationId,
      messages.Select(m => m.ToMessageDto(files)).ToList()
    );
  }

  public static PaymentDto ToPaymentDto(this Payment payment)
  {
    return new PaymentDto
    {
      PaymentId = payment.PaymentId,
      SenderId = payment.SenderId,
      ReceiverId = payment.ReceiverId,
      Amount = payment.Amount,
      PaymentStatus = payment.PaymentStatus,
      PaymentProvider = payment.PaymentProvider
    };
  }
}
