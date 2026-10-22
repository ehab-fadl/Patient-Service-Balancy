namespace PatientService.Services.DTOs.Diagnosis;

public class DiagnosisCreateDto
{
    public Guid PatientId { get; set; }
    public string MainType { get; set; } = string.Empty;
    public string SubType { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? Notes { get; set; }
}