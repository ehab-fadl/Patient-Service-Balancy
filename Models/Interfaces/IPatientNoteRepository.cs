using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPatientNoteRepository : IGenericRepository<PatientNote>
{
    Task<IEnumerable<PatientNote>> GetByPatientIdAsync(Guid patientId);
    Task<PatientNote?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}