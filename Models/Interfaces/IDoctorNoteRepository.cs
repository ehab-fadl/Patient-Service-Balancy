using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IDoctorNoteRepository : IGenericRepository<DoctorNote>
{
    Task<IEnumerable<DoctorNote>> GetByPatientIdAsync(Guid patientId);
    Task<DoctorNote?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}