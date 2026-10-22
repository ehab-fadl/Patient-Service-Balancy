using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IGad7AssessmentRepository : IGenericRepository<Gad7Assessment>
{
    Task<IEnumerable<Gad7Assessment>> GetByPatientIdAsync(Guid patientId);
    Task<Gad7Assessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}