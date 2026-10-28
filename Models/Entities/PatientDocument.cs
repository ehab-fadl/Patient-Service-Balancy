using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class PatientDocument : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // معلومات الوثيقة
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // lab, rx, report, imaging...
    public string SourceType { get; set; } = string.Empty; // internal, external
    public DateTime DocumentDate { get; set; }
    public string? Issuer { get; set; }
    public string? Category { get; set; }
    public string? RefNo { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public bool IsSigned { get; set; }
    public bool IsCritical { get; set; }
    public bool IsFavorite { get; set; } = false;
    public string? Notes { get; set; }
    public string? Tags { get; set; } // CSV

    // الملفات
    public string? FileName { get; set; }
    public string? FilePath { get; set; } // المسار على السيرفر
    public long FileSize { get; set; } // بالبايت

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}