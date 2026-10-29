using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class SystemAlert : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // نوع التنبيه (مفتاح فريد لكل قاعدة)
    // cssrs-high, rapid-deterioration, appointment-today, overdue-visit, etc.
    public string RuleId { get; set; } = string.Empty;

    // مستوى الخطورة
    public string Level { get; set; } = "info"; // critical, warning, info

    // المحتوى
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ActionLabel { get; set; } = string.Empty;

    // الحالة
    public bool IsDismissed { get; set; } = false;
    public DateTime? DismissedAt { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}