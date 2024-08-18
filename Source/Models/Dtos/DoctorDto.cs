using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public record CreateDoctorDto
{
    public required User User { get; init; }
    public required string Qualifications { get; init; }
    public required string Biography { get; init; }
    public DoctorStatus DoctorStatus { get; init; } = DoctorStatus.Active;
}

/// <summary>
/// A User with Doctor properties joined
/// </summary>
public record DoctorUser
{
    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [Phone]
    [MinLength(4, ErrorMessage = "The field must be at least 4 characters long.")]
    public required string Phone { get; init; }

    [Required]
    public required Gender Gender { get; init; }

    [Required]
    public required DateTime DateOfBirth { get; init; }

    [Required]
    public required string Address { get; init; }

    // If the user is a doctor, they may/may-not specify the following
    // Note the following fields MUST be validated in the controller based on the Role field provided as payload
    public List<string> Specialities { get; set; } = [];
    public string? Qualifications { get; set; }
    public string? Biography { get; set; }
    public DoctorStatus DoctorStatus { get; set; } = DoctorStatus.Active;
}
