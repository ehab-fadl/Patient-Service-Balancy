namespace PatientService.Services.DTOs.MdqAssessment;

public class MdqAssessmentCreateDto
{
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}