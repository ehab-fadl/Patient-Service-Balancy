namespace PatientService.Services.DTOs.SdsAssessment;

public class SdsAssessmentUpdateDto
{
    public Guid Id { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}