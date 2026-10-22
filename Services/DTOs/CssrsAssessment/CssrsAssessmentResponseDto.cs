namespace PatientService.Services.DTOs.CssrsAssessment;

public class CssrsAssessmentResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public string? AnswersJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}