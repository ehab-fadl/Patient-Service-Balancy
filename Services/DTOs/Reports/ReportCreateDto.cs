using System.Text.Json;

namespace PatientService.Services.DTOs.Reports;

public class ReportCreateDto
{
    public Guid PatientId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }

    // البيانات الديناميكية التي أدخلها الطبيب يدوياً
    public Dictionary<string, object>? FormData { get; set; }
    public string? Notes { get; set; }
}