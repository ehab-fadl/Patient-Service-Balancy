using PatientService.Models.Entities;

namespace PatientService.Services.Factories;

public interface IDepressionStoryFactory
{
    DepressionStory CreateEmptyFor(Guid patientId);
    void UpdateFrom(DepressionStory entity, string newStory, bool markAsReviewed);
}