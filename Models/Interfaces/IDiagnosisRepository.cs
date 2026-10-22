using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IDiagnosisRepository : IGenericRepository<Diagnosis>
{
    Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(Guid patientId);
    Task<Diagnosis?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}