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

        public static CreatePatientDto ToCreatePatientDto(this RegisterUserDto registerUserDto)
        {
            return new CreatePatientDto
            {
                User = registerUserDto.ToUser(),
                EmergencyContactName = registerUserDto.EmergencyContactName,
                EmergencyContactPhone = registerUserDto.EmergencyContactPhone,
                MedicalHistory = registerUserDto.MedicalHistory
            };
        }

        public static CreateDoctorDto ToCreateDoctorDto(this RegisterUserDto registerUserDto)
        {
            return new CreateDoctorDto
            {
                Biography = registerUserDto.Biography ?? "None",
                Qualifications = registerUserDto.Qualifications ?? "None",
                User = registerUserDto.ToUser()
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

        public static CreateAdminDto ToCreateAdminDto(this RegisterUserDto registerUserDto)
        {
            return new CreateAdminDto { User = registerUserDto.ToUser() };
        }
    }
}
