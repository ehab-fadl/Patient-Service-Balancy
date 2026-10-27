using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Visit : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string Reason { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; } = string.Empty;     // أولى، متابعة، طارئة، استشارة سريعة
    public string VisitLocation { get; set; } = string.Empty; // عيادة، منزل، أونلاين، مستشفى
    public string? Notes { get; set; }

    // حقل لحفظ الموعد القادم (إن وجد)
    public DateTime? NextVisitDate { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}