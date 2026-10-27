using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IVisitRepository : IGenericRepository<Visit>
{
    Task<IEnumerable<Visit>> GetByPatientIdAsync(Guid patientId);
    Task<Visit?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}