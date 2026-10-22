namespace PatientService.Services.DTOs.ChronicDisease;

public class ChronicDiseaseCreateDto
{
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }
    public string Medications { get; set; } = string.Empty;
    public string? Notes { get; set; }
}