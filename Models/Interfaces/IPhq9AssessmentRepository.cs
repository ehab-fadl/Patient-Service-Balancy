using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPhq9AssessmentRepository : IGenericRepository<Phq9Assessment>
{
    Task<IEnumerable<Phq9Assessment>> GetByPatientIdAsync(Guid patientId);
    Task<Phq9Assessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}