namespace PatientService.Services.DTOs.SdsAssessment;

public class SdsAssessmentCreateDto
{
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}