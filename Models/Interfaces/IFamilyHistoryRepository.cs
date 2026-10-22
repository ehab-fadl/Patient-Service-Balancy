using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IFamilyHistoryRepository : IGenericRepository<FamilyHistory>
{
    Task<IEnumerable<FamilyHistory>> GetByPatientIdAsync(Guid patientId);
    Task<FamilyHistory?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}