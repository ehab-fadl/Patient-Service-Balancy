using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class Medication : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string IntakePeriod { get; set; } = string.Empty; // صباحاً، ظهراً، مساءً، ليلاً
    public string Route { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = string.Empty; // نشط، متوقف، مكتمل، معلق مؤقتاً
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}