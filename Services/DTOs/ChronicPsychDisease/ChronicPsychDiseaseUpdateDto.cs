namespace PatientService.Services.DTOs.ChronicPsychDisease;

public class ChronicPsychDiseaseUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DiagnosisDate { get; set; }
    public string Hospitalization { get; set; } = string.Empty;
    public string Medications { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public string? Notes { get; set; }
}