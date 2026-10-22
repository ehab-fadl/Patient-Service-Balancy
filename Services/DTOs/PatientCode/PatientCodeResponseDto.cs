namespace PatientService.Services.DTOs.PatientCode;

public class PatientCodeResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public bool IsActive { get; set; }
}