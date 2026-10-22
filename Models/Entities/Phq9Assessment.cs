using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Phq9Assessment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty; // minimal, mild, moderate, moderately_severe, severe
    public string? AnswersJson { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}