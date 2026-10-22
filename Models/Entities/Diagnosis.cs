using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Diagnosis : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string MainType { get; set; } = string.Empty;
    public string SubType { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }
    public string Severity { get; set; } = string.Empty; // بسيط، متوسط، شديد، حرج
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}