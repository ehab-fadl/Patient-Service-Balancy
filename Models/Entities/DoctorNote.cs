using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class DoctorNote : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    // المعلومات الأساسية
    public string Title { get; set; } = string.Empty;
    public DateTime NoteDate { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Author { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;

    // التقييم الطبي
    public string? Priority { get; set; }
    public string? RiskLevel { get; set; }
    public string? Icd10 { get; set; }
    public int MoodAtNote { get; set; }
    public string Visibility { get; set; } = "خاص";
    public string? Symptoms { get; set; } // CSV

    // الربط السياقي
    public string? LinkType { get; set; }
    public Guid? LinkItemId { get; set; }

    // المحتوى
    public string Content { get; set; } = string.Empty;
    public string? NextPlan { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}