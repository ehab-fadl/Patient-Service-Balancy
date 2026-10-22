using PatientService.Services.DTOs.PatientCode;

namespace PatientService.Services.DTOs.PatientPersonalInfo;

public class PatientPersonalInfoResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? MaritalStatus { get; set; }
    public Guid DoctorId { get; set; }

    public PatientCodeResponseDto? PatientCode { get; set; }

    // ✅ معلومات مختصرة عن قصة الاكتئاب
    public bool HasDepressionStory { get; set; }
    public int DepressionStoryVersion { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}