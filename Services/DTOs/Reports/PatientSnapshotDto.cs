namespace PatientService.Services.DTOs.Reports;

public class PatientSnapshotDto
{
    public Guid PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    // بيانات إضافية يمكن جلبها من الجداول الأخرى
    public string? CurrentDiagnosis { get; set; }
    public List<string> CurrentMedications { get; set; } = new();
}