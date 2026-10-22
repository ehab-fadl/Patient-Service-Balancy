using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class CssrsAssessment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty; // low, moderate, high, critical
    public bool IsCritical { get; set; } // مؤشر سريع للخطر الحرج
    public string? AnswersJson { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}