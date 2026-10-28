using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class PatientReport : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // معلومات التقرير
    public string ReportType { get; set; } = string.Empty; // medical, consent, etc.
    public string ReportTitle { get; set; } = string.Empty;
    public bool IsLegal { get; set; }

    // معلومات المريض (Snapshot)
    public string PatientNameSnapshot { get; set; } = string.Empty;
    public DateTime? PatientDobSnapshot { get; set; }
    public string? PatientGenderSnapshot { get; set; }
    public string? PatientDiagnosisSnapshot { get; set; } // التشخيص الحالي (إن وجد)

    // البيانات الديناميكية (حقول النموذج)
    public string FormDataJson { get; set; } = "{}"; // JSON
    public string? Notes { get; set; }

    // الحالة والتواريخ
    public string Status { get; set; } = "Draft"; // Draft, Signed, Archived
    public DateTime DocumentDate { get; set; }
    public bool IsFavorite { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}