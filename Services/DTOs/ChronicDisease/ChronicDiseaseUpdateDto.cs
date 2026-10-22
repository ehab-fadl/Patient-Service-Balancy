namespace PatientService.Services.DTOs.ChronicDisease;

public class ChronicDiseaseUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }
    public string Medications { get; set; } = string.Empty;
    public string? Notes { get; set; }
}