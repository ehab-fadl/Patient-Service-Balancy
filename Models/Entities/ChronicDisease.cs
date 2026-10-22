using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class ChronicDisease : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    /// <summary>اسم المرض المزمن (مثال: ارتفاع ضغط الدم، السكري)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>تاريخ تشخيص المرض</summary>
    public DateTime DiagnosisDate { get; set; }

    /// <summary>الأدوية المستخدمة لعلاج المرض</summary>
    public string Medications { get; set; } = string.Empty;

    /// <summary>ملاحظات طبية إضافية (اختياري)</summary>
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}