namespace PatientService.Services.DTOs.DepressionStory;

public class DepressionStoryResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Story { get; set; } = string.Empty;
    public int StoryVersion { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}