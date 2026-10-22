using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IMedicationRepository : IGenericRepository<Medication>
{
    Task<IEnumerable<Medication>> GetByPatientIdAsync(Guid patientId);
    Task<Medication?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}