using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class PatientCode : BaseEntity
{
    public Guid PatientId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}