using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IListeningSessionRepository : IGenericRepository<ListeningSession>
{
    Task<IEnumerable<ListeningSession>> GetByPatientIdAsync(Guid patientId);
    Task<ListeningSession?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}