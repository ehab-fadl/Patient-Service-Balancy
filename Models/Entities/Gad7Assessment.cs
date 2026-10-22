using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Gad7Assessment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty; // minimal, mild, moderate, severe
    public string? AnswersJson { get; set; } // JSON string storing the raw answers

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}