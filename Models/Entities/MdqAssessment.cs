using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class MdqAssessment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty; // positive, negative
    public string? AnswersJson { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}