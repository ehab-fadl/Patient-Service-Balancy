using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPatientDocumentRepository : IGenericRepository<PatientDocument>
{
    Task<IEnumerable<PatientDocument>> GetByPatientIdAsync(Guid patientId);
    Task<PatientDocument?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}