using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class PatientPersonalInfo : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? MaritalStatus { get; set; }

   
    public Guid DoctorId { get; set; }
    public PatientCode? PatientCode { get; set; }
    public DepressionStory? DepressionStory { get; set; }
}