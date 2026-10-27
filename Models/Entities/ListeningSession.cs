using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class ListeningSession : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // المعلومات الأساسية
    public string Title { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public TimeSpan? SessionTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? SessionType { get; set; }
    public string Status { get; set; } = string.Empty;

    // تفاصيل العلاج
    public string? TherapyType { get; set; }
    public string? Format { get; set; }
    public string? TherapistName { get; set; }
    public Guid? LinkedTreatmentPlanId { get; set; } // ربط بخطة علاجية (اختياري)

    // قياس المزاج
    public int MoodBefore { get; set; } // 0-10
    public int MoodAfter { get; set; }  // 0-10

    // المحتوى السريري (يُخزن كـ CSV)
    public string? TopicsDiscussed { get; set; }
    public string? TechniquesUsed { get; set; }

    // تقييم الخطر والمهام
    public string? RiskLevel { get; set; }
    public bool HomeworkCompleted { get; set; }
    public string? HomeworkAssigned { get; set; }
    public string? NextSessionPlan { get; set; }

    // ملاحظات
    public string? Notes { get; set; }
    public string? ClinicalNotes { get; set; }

    // Navigation Properties
    public PatientPersonalInfo Patient { get; set; } = null!;
}