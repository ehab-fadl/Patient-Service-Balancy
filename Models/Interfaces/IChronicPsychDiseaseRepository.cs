using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IChronicPsychDiseaseRepository : IGenericRepository<ChronicPsychDisease>
{
    Task<IEnumerable<ChronicPsychDisease>> GetByPatientIdAsync(Guid patientId);
    Task<ChronicPsychDisease?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}