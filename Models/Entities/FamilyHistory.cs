using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class FamilyHistory : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string Relation { get; set; } = string.Empty;
    public string? RelativeName { get; set; }
    public int? Age { get; set; }
    public string? Status { get; set; }       // حي / متوفى / غير معروف
    public string Severity { get; set; } = string.Empty; // unknown / low / medium / high
    public string ConditionType { get; set; } = string.Empty; // genetic / psych / chronic / other

    public string Conditions { get; set; } = string.Empty; // CSV: "السكري، ارتفاع ضغط الدم"
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}