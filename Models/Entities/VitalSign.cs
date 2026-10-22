using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class VitalSign : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public string VitalType { get; set; } = string.Empty; // bloodPressure, heartRate, weight...
    public DateTime MeasurementDate { get; set; }

    public decimal Value1 { get; set; }
    public decimal? Value2 { get; set; } // للضغط فقط (الانبساطي)

    public string Status { get; set; } = string.Empty; // normal, warning, danger
    public string? Notes { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}