using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class DepressionStory : BaseEntity
{
    public Guid PatientId { get; set; }
    public string Story { get; set; } = string.Empty;
    public int StoryVersion { get; set; }
    public DateTime? LastReviewedAt { get; set; }

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}