namespace PatientService.Services.DTOs.Gad7Assessment;

public class Gad7AssessmentCreateDto
{
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}