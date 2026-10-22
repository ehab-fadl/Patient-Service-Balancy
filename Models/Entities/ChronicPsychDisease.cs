using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class ChronicPsychDisease : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }

    // "نعم" أو "لا"
    public string Hospitalization { get; set; } = string.Empty;

    public string Medications { get; set; } = string.Empty;

    // "بسيط", "متوسط", "شديد", "حرج"
    public string Severity { get; set; } = string.Empty;

    // "ممتازة", "جيدة", "جزئية", "ضعيفة / مقاومة"
    public string Response { get; set; } = string.Empty;

    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}