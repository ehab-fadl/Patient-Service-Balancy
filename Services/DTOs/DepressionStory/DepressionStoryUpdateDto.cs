namespace PatientService.Services.DTOs.DepressionStory;

public class DepressionStoryUpdateDto
{
    public Guid PatientId { get; set; }
    public string Story { get; set; } = string.Empty;
    public bool MarkAsReviewed { get; set; }
}