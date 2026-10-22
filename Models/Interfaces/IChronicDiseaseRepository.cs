using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IChronicDiseaseRepository : IGenericRepository<ChronicDisease>
{
    Task<IEnumerable<ChronicDisease>> GetByPatientIdAsync(Guid patientId);
    Task<ChronicDisease?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
    Task<int> CountByPatientAsync(Guid patientId);
}