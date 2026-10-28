namespace PatientService.Services.DTOs.Reports;

public class ReportResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string ReportTitle { get; set; } = string.Empty;
    public bool IsLegal { get; set; }

    public DateTime DocumentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }

    // بيانات المريض (Snapshot)
    public string PatientName { get; set; } = string.Empty;
    public DateTime? PatientDob { get; set; }
    public string? PatientGender { get; set; }
    public string? PatientDiagnosis { get; set; }

    // البيانات الديناميكية
    public string FormDataJson { get; set; } = "{}";
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}