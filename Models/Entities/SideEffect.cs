using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class SideEffect : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string MedicationName { get; set; } = string.Empty;
    public string Symptom { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}