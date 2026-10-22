namespace PatientService.Services.DTOs.Phq9Assessment;

public class Phq9AssessmentUpdateDto
{
    public Guid Id { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}