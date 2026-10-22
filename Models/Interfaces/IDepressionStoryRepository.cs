using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IDepressionStoryRepository : IGenericRepository<DepressionStory>
{
    Task<DepressionStory?> GetByPatientIdAsync(Guid patientId);
    Task<bool> ExistsForPatientAsync(Guid patientId);
}