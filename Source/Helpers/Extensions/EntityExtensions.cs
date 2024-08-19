using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;

namespace HealthHub.Source.Helpers.Extensions
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

        public static CreatePatientDto ToCreatePatientDto(
            this RegisterUserDto registerUserDto,
            User user
        )
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
            User user
        )
        {
            return new CreateDoctorDto
            {
                Biography = registerUserDto.Biography ?? "None",
                Qualifications = registerUserDto.Qualifications ?? "None",
                User = user
            };
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
            return new RegisterUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = password,
                Phone = user.Phone,
                Gender = user.Gender,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address
            };
        }

        public static CreateAdminDto ToCreateAdminDto(
            this RegisterUserDto registerUserDto,
            User user
        )
        {
            return new CreateAdminDto { User = user };
        }

        public static DoctorDto ToDoctorDto(this Doctor d)
        {
            return new DoctorDto
            {
                UserId = d.UserId,
                DoctorId = d.DoctorId,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Email = d.User.Email,
                IsEmailVerified = d.User.IsEmailVerified,
                Phone = d.User.Phone,
                Gender = d.User.Gender,
                DateOfBirth = d.User.DateOfBirth,
                Address = d.User.Address,
                Specialities = d
                    .DoctorSpecialities.Select(ds => ds.Speciality.SpecialityName)
                    .ToList(),
                Qualifications = d.Qualifications,
                Biography = d.Biography,
                DoctorStatus = d.DoctorStatus,
                ProfilePicture = d.User.ProfilePicture ?? ""
            };
        }

        public static PatientDto ToPatientDto(this Patient patient)
        {
            return new PatientDto
            {
                UserId = patient.UserId,
                PatientId = patient.PatientId,
                FirstName = patient.User.FirstName,
                LastName = patient.User.LastName,
                Email = patient.User.Email,
                IsEmailVerified = patient.User.IsEmailVerified,
                Phone = patient.User.Phone,
                Gender = patient.User.Gender,
                DateOfBirth = patient.User.DateOfBirth,
                Address = patient.User.Address,
                EmergencyContactName = patient.EmergencyContactName ?? "",
                EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
                MedicalHistory = patient.MedicalHistory ?? "",
                ProfilePicture = patient.User.ProfilePicture ?? "",
            };
        }
    }
}
