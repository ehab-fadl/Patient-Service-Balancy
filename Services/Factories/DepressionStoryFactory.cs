using PatientService.Models.Entities;

namespace PatientService.Services.Factories;

public class DepressionStoryFactory : IDepressionStoryFactory
{
    public DepressionStory CreateEmptyFor(Guid patientId)
    {
        return new DepressionStory
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Story = string.Empty,
            StoryVersion = 1,
            LastReviewedAt = null
        };
    }

    public void UpdateFrom(DepressionStory entity, string newStory, bool markAsReviewed)
    {
        entity.Story = newStory?.Trim() ?? string.Empty;

       
        if (entity.Story.Length > 0)
        {
            entity.StoryVersion++;
        }

        if (markAsReviewed)
        {
            entity.LastReviewedAt = DateTime.UtcNow;
        }
    }
}