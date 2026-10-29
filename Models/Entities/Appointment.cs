using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // معلومات الجدولة
    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; } // "14:30"

    // المعلومات
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string VisitType { get; set; } = "متابعة";
    public string VisitLocation { get; set; } = "عيادة";

    // الحالة (مهم جداً)
    public string Status { get; set; } = "Pending";
    // Pending, Confirmed, Completed, Cancelled, NoShow

    // عند الإلغاء
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }

    // عند التحويل إلى زيارة فعلية
    public Guid? VisitId { get; set; }

    // التذكيرات
    public DateTime? ReminderSentAt { get; set; }
    public bool ReminderRequested { get; set; } = false;

    // Navigation Properties
    public PatientPersonalInfo Patient { get; set; } = null!;
    public Visit? Visit { get; set; }
}