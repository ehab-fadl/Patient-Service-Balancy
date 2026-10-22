using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IVitalSignRepository : IGenericRepository<VitalSign>
{
    Task<IEnumerable<VitalSign>> GetByPatientIdAsync(Guid patientId);
    Task<VitalSign?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}