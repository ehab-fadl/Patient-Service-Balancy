using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class TreatmentPlan : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty; // نشطة، مكتملة، ملغاة
    public string? PlanDoctorName { get; set; } // اسم الطبيب المسؤول (اختياري)
    public string? Notes { get; set; }

    // Navigation Properties
    public PatientPersonalInfo Patient { get; set; } = null!;
    public ICollection<TreatmentProblem> Problems { get; set; } = new List<TreatmentProblem>();
}